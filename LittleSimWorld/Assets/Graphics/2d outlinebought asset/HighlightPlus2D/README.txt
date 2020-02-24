**************************************
*          HIGHLIGHT PLUS 2D         *
* Created by Ramiro Oliva (Kronnect) * 
*            README FILE             *
**************************************


How to use this asset
---------------------

1) Highlighting/customizing specific sprites:

- Add HighlightEffect.cs script to any sprite. Customize the appearance options.
- Optionally add HighlightTrigger.cs script to the sprite GameObject. It will activate highlight on the sprite when mouse pass over it. Note that a collider must be present on the sprite.
(adding a HighlightTrigger.cs script to an sprite will automatically add a HighlightEffect component automatically)


2) Highlighting any sprite automatically:

- Select top menu GameObject -> Effects -> Highlight Plus 2D -> Create Manager.
- Customize behaviour of Highlight Manager 2D. If a sprite already has a HighlightEffect component, it will use those settings.


3) Ignoring specific sprites from highlighting:

- Add a HighlightEffect component to the sprite and enable "Ignore" checkbox under Highlight section.



Events
------

When an object is highlighted you can use the HighlightStart or HighlightEnd events. Check PotionHighlightEventSample.cs script in the demo scene for an example.



Demo scene
----------
All icons in the demo scene are Public Domain.



Support & Feedback
------------------

If you like Highlight Plus 2D, please rate it on the Asset Store. It encourages us to keep improving it! Thanks!

Have any question or issue?
* Email: contact@kronnect.me
* Support Forum: http://kronnect.me
* Twitter: @KronnectGames



Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of Highlight Plus 2D will be eventually available on the Asset Store.



More Cool Assets!
-----------------
Check out our other assets here:
https://assetstore.unity.com/publishers/15018



Version history
---------------

Version 2.3
- Added "Outline Exclusive" option (shows outline regardless of other overlapping highlighted sprites)

Version 2.2.1
- Improved outline effect when Smooth Edge is used on semi-transparent sprites
- Minor fixes
- [Fix] Fixed demo sprite sheet bleeding issue

Version 2.1
- Improved "Smooth Edges" option
- Outline / Glow quality now offers 3 levels

Version 2.0
- Added support for polygon packed sprites
- Added "Render On Top" option to glow and outline
- Added "Smooth Edges" option to glow and outline
- [Fix] Fixed alpha issue with 2D shadow effect 

Version 1.8
- Added "Outline Always On Top" and "Glow Always On Top" options

Version 1.7.1
- [Fix] Fixed non-uniform outline widths in different parts of a hierarchy of sprites

Version 1.7
- Added "Scale" option
- [Fix] Fixed occluder option not rendering when no other effect is enabled

Version 1.6
- Added "Occluder" option. Forces sprite to write to z-buffer causing occlusion to other sprites (enables see-through)
- [Fix] Fixed demo scene for LWRP

Version 1.5
- Added "Shadow" effect

Version 1.4
- Changed rendering method to support outline occlusion
- Added compatibility with LWRP (Lightweight Rendering Pipeline)
- Added "Highlight Event" and "Highlight Duration" to trigger and manager
- Added "Overlay RenderQueue" parameter
- API: added OnObjectHighlightStart, ObObjectHighlightEnd events to Highlight Manager 2D component

Version 1.3
- Added "Raycast Source" to Highlight Trigger and Manager

Version 1.2
- Added support for quad-based sprites

Version 1.1
- Added "Overlay Min Intensity" and "Overlay Blending" options
- Added "Glow HQ" option
- [Fix] Clicking Reset command does not refresh properly

Version 1.0
- Initial release
