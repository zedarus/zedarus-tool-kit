# Hooking Up To Unity Project

Clone as a submodule into your Unity project, but outside of the Assets folder.

Create following symlinks:

## Important

To make symlinks work on both platforms, create them on windows with `mklink /d` command (make sure to follow these instructions: https://github.com/git-for-windows/git/wiki/Symbolic-Links)

## OS X

```
ln -s ~/<path-to-repo>/Assets/Plugins/ZTK ./Assets/Plugins/ZTK
Example:
ln -s ../../../../../Submodules/ZTK/ZedarusToolKit/Assets/Plugins/ ZTK
```

## Windows

```
mklink /d <local_link> <relative_link_to_ZTK>
Example: 
mklink /d ZTK ..\..\..\..\..\Submodules\ZTK\ZedarusToolKit\Assets\Plugins\
```
