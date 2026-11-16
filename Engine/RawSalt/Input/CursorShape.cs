namespace RawSalt.Input;

/// <summary>
/// The shape of the mouse cursor.
/// </summary>
/// <remarks>
/// Values follow the shared cursor-shape names used by Wayland's <c>wp_cursor_shape_device_v1</c>.
/// Platforms without cursor shape support can ignore changes.
/// </remarks>
public enum CursorShape
{
	Default,
	ContextMenu,
	Help,
	Pointer,
	Progress,
	Wait,
	Cell,
	Crosshair,
	Text,
	VerticalText,
	Alias,
	Copy,
	Move,
	NoDrop,
	NotAllowed,
	Grab,
	Grabbing,
	ResizeEast,
	ResizeNorth,
	ResizeNorthEast,
	ResizeNorthWest,
	ResizeSouth,
	ResizeSouthEast,
	ResizeSouthWest,
	ResizeWest,
	ResizeEastWest,
	ResizeNorthSouth,
	ResizeNorthEastSouthWest,
	ResizeNorthWestSouthEast,
	ResizeColumn,
	ResizeRow,
	AllScroll,
	ZoomIn,
	ZoomOut,
	DragAndDropAsk,
	AllResize,
}
