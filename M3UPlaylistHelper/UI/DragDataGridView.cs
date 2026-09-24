namespace M3UPlaylistHelper.UI;

/// <summary>
/// A DataGridView whose rows can be dragged. Like Explorer, pressing the mouse on a row that is part of a multi-row
/// selection keeps the selection (so all of it can be dragged) and only selects the single row on mouse up.
/// </summary>
public class DragDataGridView : DataGridView
{
    private Rectangle dragBox = Rectangle.Empty;
    private MouseEventArgs? deferredMouseDown;

    /// <summary>
    /// Returns the data to drag, or null to not start a drag. Called once the mouse moved far enough with the button down.
    /// </summary>
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Func<object?>? DragDataProvider { get; set; }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        dragBox = Rectangle.Empty;
        deferredMouseDown = null;

        if (e.Button == MouseButtons.Left && EditingControl == null && DragDataProvider != null)
        {
            var hit = HitTest(e.X, e.Y);

            // Check boxes keep working as normal, dragging starts from the other cells
            if (hit.Type == DataGridViewHitTestType.Cell && hit.RowIndex >= 0 && Columns[hit.ColumnIndex] is not DataGridViewCheckBoxColumn)
            {
                var size = SystemInformation.DragSize;
                dragBox = new Rectangle(e.X - size.Width / 2, e.Y - size.Height / 2, size.Width, size.Height);

                if (Rows[hit.RowIndex].Selected && SelectedRows.Count > 1 && ModifierKeys == Keys.None)
                {
                    deferredMouseDown = e;
                    Focus();
                    return;
                }
            }
        }

        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && dragBox != Rectangle.Empty && !dragBox.Contains(e.Location))
        {
            dragBox = Rectangle.Empty;
            deferredMouseDown = null;

            if (DragDataProvider?.Invoke() is object data)
            {
                DoDragDrop(data, DragDropEffects.Move);
                return;
            }
        }

        base.OnMouseMove(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        dragBox = Rectangle.Empty;

        // It was a click, not a drag: select just the clicked row now
        if (deferredMouseDown != null)
        {
            var mouseDown = deferredMouseDown;
            deferredMouseDown = null;
            base.OnMouseDown(mouseDown);
        }

        base.OnMouseUp(e);
    }
}
