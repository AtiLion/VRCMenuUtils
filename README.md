# VRCMenuUtils
A mod library that makes integrating into the VRChat menu system easier!

## How to use
In order to utilize VRCMenuUtils, it must be loaded first. You can wait for it with:
```csharp
yield return VRCMenuUtils.WaitForInit();
```
after which you can take advantage of its functions.
To create a new user info button, you need to create a new VRCEUiButton, and assign it your OnClick event. This can be done as follows:
```csharp
VRCEUiButton newButton = new VRCEUiButton("Example Name", new Vector2(0f, 0f), "Button Text");
newButton.Button.onClick.AddListener(() =>
{
  exampleFunction()
});
```
Afterwards, you can add it to the User Info page like this:
```csharp
VRCMenuUtils.AddUserInfoButton(newButton);
```
For a QuickMenu button, you need to create a new VRCEUiQuickButton:
```csharp
VRCEUiQuickButton newQMButton = new VRCEUiQuickButton("Example Name", new Vector2(0f, 0f), "Button\nText", "This text will appear in the tooltip of the Quick Menu", null)
newQMButton.Button.onClick.AddListener(() =>
{
  exampleQMFunction();
});
```
Afterwards, you can add a Quick Menu button as follows:
```csharp
VRCMenuUtils.AddQuickMenuButton(newQMButton);
```
