# UnityAssetBundler
```sh
$ git clone git@github.com:gin0606/UnityAssetBundler.git
$ cd UnityAssetBundler
$ /Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -projectPath $(pwd) -executeMethod Bundler.ExportAll -quit
```

```sh
$ tree Assets/Resources
Assets/Resources
├── tabemono1
│   ├── niku1
│   │   └── 2012-02-10\ 13.26.21.jpg
│   └── raamen1
│       └── 2012-02-10\ 20.04.33.jpg
├── tabemono2
│   ├── niku2
│   │   └── 2012-02-10\ 13.26.21.jpg
│   └── raamen2
│       └── 2012-02-10\ 20.04.33.jpg
└── tabemono_sonomama
    ├── niku_sonomama
    │   └── 2012-02-10\ 13.26.21.jpg
    └── raamen_sonomama
        └── 2012-02-10\ 20.04.33.jpg
```

```sh
$ tree build
build
├── tabemono1
│   ├── Android
│   │   ├── niku1.unity3d
│   │   └── raamen1.unity3d
│   ├── StandaloneOSXUniversal
│   │   ├── niku1.unity3d
│   │   └── raamen1.unity3d
│   └── iPhone
│       ├── niku1.unity3d
│       └── raamen1.unity3d
├── tabemono2
│   ├── Android
│   │   ├── niku2.unity3d
│   │   └── raamen2.unity3d
│   ├── StandaloneOSXUniversal
│   │   ├── niku2.unity3d
│   │   └── raamen2.unity3d
│   └── iPhone
│       ├── niku2.unity3d
│       └── raamen2.unity3d
└── tabemono_sonomama
    ├── Android
    │   ├── niku_sonomama.unity3d
    │   └── raamen_sonomama.unity3d
    ├── StandaloneOSXUniversal
    │   ├── niku_sonomama.unity3d
    │   └── raamen_sonomama.unity3d
    └── iPhone
        ├── niku_sonomama.unity3d
        └── raamen_sonomama.unity3d
```
