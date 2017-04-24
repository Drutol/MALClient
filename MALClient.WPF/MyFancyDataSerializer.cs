using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace MALClient.WPF
{
    public class MyFancyDataSerializer
    {
        private const int DelimeterLength = 6;
        private const string DelimeterSequence = "<>@$<>";
        private const string StartType = ">>#@>>";
        private const string EndType = "<<@#<<";
        private const string StartProperty = ">>!@<<";
        private const string EndProperty = "<<@!>>";

        private readonly List<(Guid guid, PropertyInfo info, object owner, Type type)>
            _deserailizedObjectReferenceDelayedResolutions =
                new List<(Guid guid, PropertyInfo info, object owner, Type type)>();

        private readonly Dictionary<Type, Dictionary<Guid, object>> _deserializedObjectReferenceLookup =
            new Dictionary<Type, Dictionary<Guid, object>>();

        private readonly Dictionary<Type, Dictionary<int, Guid>> _referenceLookupDictionary =
            new Dictionary<Type, Dictionary<int, Guid>>();

        public string SerializeObject(object data, Type type)
        {
            var output = SerializeObjectInner(data, type);
            output = output.Substring(output.IndexOf(DelimeterSequence) + DelimeterLength);
            var lastIndex = output.LastIndexOf(type.AssemblyQualifiedName);
            output = output.Substring(0, lastIndex);
            return output;
        }

        public string SerializeObjectInner(object data, Type type)
        {
            var output = $"{StartType}{type.AssemblyQualifiedName}{DelimeterSequence}";
            if (data == null)
                return $"{output}|nil{DelimeterSequence}";

            var hash = data.GetHashCode();
            if (_referenceLookupDictionary.ContainsKey(type))
                if (_referenceLookupDictionary[type].ContainsKey(hash))
                    return
                        $"{_referenceLookupDictionary[type][hash]}{DelimeterSequence}{type.AssemblyQualifiedName}{EndType}";


            var members = type.GetProperties();

            if (Attribute.IsDefined(type, typeof(DataContractAttribute)))
                members = members.Where(info => Attribute.IsDefined(info, typeof(DataMemberAttribute))).ToArray();

            foreach (var memberInfo in members.OrderByDescending(info => Type.GetTypeCode(info.PropertyType)))
            {
                var code = Type.GetTypeCode(memberInfo.PropertyType);
                switch (code)
                {
                    case TypeCode.Boolean:
                    case TypeCode.Int32:
                    case TypeCode.String:
                        output +=
                            $"{StartProperty}{(int) code}|{memberInfo.Name}|{memberInfo.GetValue(data)}{EndProperty}";
                        break;
                    default:
                    {
                        var value = memberInfo.GetValue(data);
                        var objHash = value.GetHashCode();
                        var identificatrionGuid = Guid.NewGuid();
                        if (!_referenceLookupDictionary.ContainsKey(type))
                            _referenceLookupDictionary[type] = new Dictionary<int, Guid>
                            {
                                {objHash, identificatrionGuid}
                            };
                        else if (!_referenceLookupDictionary[type].ContainsKey(objHash))
                            _referenceLookupDictionary[type].Add(objHash, identificatrionGuid);


                        switch (value)
                        {
                            case IEnumerable enumerable:
                                Type collectionType = null;
                                foreach (var item in enumerable)
                                {
                                    collectionType = item.GetType();
                                    break;
                                }
                                output +=
                                    $"{StartType}{memberInfo.PropertyType.AssemblyQualifiedName}|{memberInfo.Name}|{identificatrionGuid}|{collectionType.AssemblyQualifiedName}{DelimeterSequence}";
                                foreach (var item in enumerable)
                                    output += $"{SerializeObjectInner(item, item.GetType())}{DelimeterSequence}";
                                output += $"{memberInfo.PropertyType.AssemblyQualifiedName}{EndType}";
                                break;

                            default:
                            {
                                output +=
                                    $"{StartType}{memberInfo.PropertyType.AssemblyQualifiedName}|{memberInfo.Name}|{identificatrionGuid}{DelimeterSequence}";
                                output += $"{SerializeObjectInner(value, value.GetType())}{DelimeterSequence}";
                                output += $"{memberInfo.PropertyType.AssemblyQualifiedName}{EndType}";
                                break;
                            }
                        }
                        break;
                    }
                }
            }


            return $"{output}{DelimeterSequence}{type.AssemblyQualifiedName}{EndType}";
        }

        public object DeserializeObject(string data, Type type)
        {
            var output = DeserializeObjectInner(data, type);
            foreach (var delayedResolution in _deserailizedObjectReferenceDelayedResolutions)
                try
                {
                    delayedResolution.info.SetValue(
                        delayedResolution.owner,
                        _deserializedObjectReferenceLookup[delayedResolution.type][
                            delayedResolution.guid]);
                }
                catch (Exception e)
                {
                    //well something went terribly wrong
                }
            return output;
        }

        private object DeserializeObjectInner(string data, Type type)
        {
            var output = Activator.CreateInstance(type);
            var propertiesInfo = type.GetProperties();

            var dataCopy = data;
            while (dataCopy.Any())
            {
                var token = dataCopy.Substring(0, DelimeterLength);
                if (token == DelimeterSequence)
                {
                    dataCopy = dataCopy.Substring(DelimeterLength);
                    continue;
                }
                if (token == "MALCli" && dataCopy.Contains(EndType))
                    dataCopy = "";
                else if (dataCopy.Substring(DelimeterLength * 2) == $"{StartType}{DelimeterSequence}")
                    dataCopy.Remove(6, DelimeterLength);
                switch (token)
                {
                    case StartType:
                    {
                        string typeName;
                        try
                        {
                            typeName = dataCopy.Substring(DelimeterLength, dataCopy.IndexOf('|') - DelimeterLength);
                        }
                        catch (Exception e)
                        {
                            //end of object
                            dataCopy = "";
                            break;
                        }

                        if (CountStringOccurrences(typeName, "1.0.0.0") == 2)
                            typeName = typeName.Split(new[] {DelimeterSequence}, StringSplitOptions.RemoveEmptyEntries)
                                .First();
                        var finishingSequence = $"{typeName}{EndType}";
                        var finishingSequenceIndex = dataCopy.IndexOf(finishingSequence) + finishingSequence.Length -
                                                     DelimeterLength;
                        var typeSequence = dataCopy.Substring(DelimeterLength, finishingSequenceIndex);
                        var typeDefEndIndex = typeSequence.IndexOf(DelimeterSequence);
                        var typeDefinition = typeSequence.Substring(0, typeDefEndIndex);
                        var typeTokens = typeDefinition.Split('|');
                        var typeContents = typeSequence.Substring(typeDefEndIndex);

                        if (typeTokens.Length == 1)
                        {
                            //Get rid of header 
                            dataCopy = dataCopy.Substring(dataCopy.IndexOf(DelimeterSequence) + DelimeterLength);
                            continue;
                        }

                        object obj = null;
                        var objectBuilt = true;


                        dataCopy = dataCopy.Remove(DelimeterLength, finishingSequenceIndex);

                        var innerType = Type.GetType(typeTokens[0]);
                        var propInfo = propertiesInfo.First(info => info.Name.Equals(typeTokens[1]));

                        if (!_deserializedObjectReferenceLookup.ContainsKey(innerType))
                            _deserializedObjectReferenceLookup.Add(innerType, new Dictionary<Guid, object>());

                        if (typeContents != "nil")
                        {
                            var maybeGuid = typeContents.Split(new[] {DelimeterSequence, DelimeterSequence},
                                    StringSplitOptions.RemoveEmptyEntries)
                                .FirstOrDefault();
                            if (Guid.TryParse(maybeGuid, out Guid guid))
                            {
                                if (_deserializedObjectReferenceLookup[innerType].ContainsKey(guid))
                                {
                                    obj = _deserializedObjectReferenceLookup[innerType][guid];
                                }
                                else
                                {
                                    _deserailizedObjectReferenceDelayedResolutions.Add(
                                        (guid, propInfo, output, innerType));
                                    objectBuilt = false;
                                }
                            }
                            else
                            {
                                if (typeof(IEnumerable).IsAssignableFrom(innerType))
                                {
                                    dataCopy = dataCopy.Substring(DelimeterLength);
                                    var innerEnumerableType = Type.GetType(typeTokens[3]);
                                    var contents =
                                        typeContents.Split(
                                                new[] {$"{StartType}{typeTokens[3]}", $"{typeTokens[3]}{EndType}"},
                                                StringSplitOptions.None)
                                            .Where(s => s != DelimeterSequence);

                                    dynamic dynamicList = Activator.CreateInstance(innerType);
                                    foreach (var item in contents.Take(contents.Count() - 1))
                                    {
                                        var itemObj = DeserializeObjectInner(item.Substring(DelimeterLength),
                                            innerEnumerableType);
                                        dynamicList.Add((dynamic) itemObj);
                                    }

                                    obj = dynamicList;
                                }
                                else
                                {
                                    obj = DeserializeObjectInner(typeContents, innerType);
                                }

                                _deserializedObjectReferenceLookup[innerType][Guid.Parse(typeTokens[2])] = obj;
                            }
                        }

                        if (objectBuilt)
                        {
                            if (!_deserializedObjectReferenceLookup[innerType].ContainsKey(Guid.Parse(typeTokens[2])))
                                _deserializedObjectReferenceLookup[innerType].Add(Guid.Parse(typeTokens[2]), obj);
                            propInfo.SetValue(output, obj);
                        }


                        break;
                    }
                    // $"{StartProperty}{(int)code}:{memberInfo.Name}:{memberInfo.GetValue(data)}{DelimeterSequence}{EndProperty}";
                    case StartProperty:
                    {
                        var propEndIndex = dataCopy.IndexOf(EndProperty);
                        var typeSequence = dataCopy.Substring(DelimeterLength, propEndIndex - DelimeterLength);

                        var typeTokens = typeSequence.Split('|');
                        object value = null;
                        switch ((TypeCode) int.Parse(typeTokens[0]))
                        {
                            case TypeCode.Boolean:
                                value = bool.Parse(typeTokens[2]);
                                break;
                            case TypeCode.Int32:
                                value = int.Parse(typeTokens[2]);
                                break;
                            case TypeCode.String:
                                value = typeTokens[2];
                                break;
                        }

                        dataCopy = dataCopy.Substring(propEndIndex + DelimeterLength);


                        var propInfo = propertiesInfo.First(info => info.Name.Equals(typeTokens[1]));
                        propInfo.SetValue(output, value);
                        break;
                    }
                }
            }
            return output;
        }


        private int CountStringOccurrences(string text, string pattern)
        {
            var count = 0;
            var i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }
    }
}