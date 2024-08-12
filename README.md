Repository to reproduce an issue discovered in MAUI where an ArcSegment was periodically drawn offset from expectations.

The Arcsegment offset issue is most easily shown by toggling on Animation and then watching the flickering as you tap the Advance button.
At some breakpoints (most noticeable here at 23.1) it is off by several pixels.
![image](https://github.com/user-attachments/assets/961898da-428c-474d-a6a4-43a93255e524)

Tapping Advance adds 1.1 to the current value (0-100) and tracks the last StartPoint fed to the ArcSegment being drawn.  The starting points are collected into a line to demonstrate that the starting points are along the (natural) path of the arc.  This can show the "expected arc" if you click through the full range of values.
