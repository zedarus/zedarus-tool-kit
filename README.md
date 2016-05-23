# Zedarus Tool Kit

A bunch or reusable classes, utilities and tools that I'm using in my games in Unity.

# Important

Since this project ignores Unity's `.meta` files, you should **NEVER** add any `MonoBehaviour` classes from this project directly to your scene objects as components, because as you `GUID` for those scripts change between projects, you might have "Missing Behaviour" error.

Instead, just create a new class in your local project, extend the required class from ZTK and then add it as a component.


