# Hooking Up To Unity Project

Clone as a submodule into your Unity project, but outside of the Assets folder.

Create following symlinks:

## OS X

```
ln -s ~/<path-to-repo>/Assets/ ./Assets
```

## Windows

```
mklink /D C:\Users\<user>\AppData\Roaming\MonoDevelop-{version}\Config C:\<path-to-repo>\Config
```
