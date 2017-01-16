# MALClient
It's rather robust MyAnimeList client application interfacing with both "official" api and the website api with wild endpoints with cookies and such. Right now it's available on Windows 10 devices and android version is now in the works.

![](https://github.com/Drutol/MALClient/blob/master/MALClient.Dekstop/Assets/Square150x150Logo.scale-100.png)
<a href="https://www.microsoft.com/store/apps/9nblggh5f3bl?ocid=badge"><img src="https://assets.windowsphone.com/f2f77ec7-9ba9-4850-9ebe-77e366d08adc/English_Get_it_Win_10_InvariantCulture_Default.png" width="47.5%" alt="Get it on Windows 10" /></a>

### Screenshots (UWP)
![](http://i.imgur.com/HxFkygB.png)
### Features
- Anime and manga list updates.
 - Score,Status,Episodes,Volumes
 - Tags
 - Favourites
 - Start/End date
 - Rewatching
- Anime list with sorting, filters.
 - Grid view
 - Compact view
 - Detailed grid view
- Anime info.
 - Genres
 - Episodes
 - Reviews
 - Recommendations
 - Personalized anime/manga suggestions.
 - Related
 - Characters&Staff
 - Mal statisctics
 - Promotional videos
- Top anime/manga.
 - With multiple categories
- Seasonal anime
 - With 4 seasons to choose from
- Anime by studio and genre
- Global anime&manga recommendations
- Calendar
 - With countdowns
- Mal articles
 - Mal news
 - Live tiles
- Mal messaging 
- Tons of settings
- Mal profile
 - With navigation accross others' profiles
 - Profile comments (with send)
 - Profile converstion
- Forums
 - With dark theme
 - Semi hybrid (a bit of html and a bit of native)
 - Automatic authentication
- System toasts and notification hub!
- Friends feeds parsed from rss channels.
- And much more!

### Compilation
You should be able to compile this thing out of the box, you may have to generate certificate though.
### Code
There are a few "dreadful places" but I think this code won't damage your eyes too much. (no promises!)
I'm planning big refactoring with developing for android.

If you are looking for methods to communicate with MAL go to Comm folder where you will find all queries.

### "Protocol"

If you'd like for some reason to launch my app externally you can do so by using this protocol:
```
malclient://<your everyday MAL link>
```
List of all accepted urls can be found [here](https://github.com/Drutol/MALClient/blob/714a73a3f4389a3212843fda243c1034c7347144/MALClient.XShared/Utils/MalLinkParser.cs)
