``` powershell
cd C:\Git\myalbum\v8.0\src\MyAlbum

dotnet publish -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true `
  -o C:\Temp\MyAlbumDist
```

```powershell
cd C:\Git\myalbum\v8.0\src\FontManager

dotnet publish -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true `
  -o C:\Temp\FontManagerDist
```