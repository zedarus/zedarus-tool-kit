# Hooking Up To Unity Project

Clone as a submodule into your Unity project, but outside of the Assets folder.

Create following symlinks:

## OS X

```
ln -s ~/<path-to-repo>/Assets/Plugins/ZTK ./Assets/Plugins/ZTK
```

## Windows

```
mklink /d <local_link> <relative_link_to_ZTK>
```
