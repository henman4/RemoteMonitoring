Public Class TabControlClass
    Inherits TabControl

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        DoubleBuffered = True
        SizeMode = TabSizeMode.Fixed
        ItemSize = New Size(30, 120)
    End Sub
    Protected Overrides Sub OnMouseHover(ByVal e As System.EventArgs)
        MyBase.OnMouseHover(e)
    
    End Sub
    Protected Overrides Sub CreateHandle()
        MyBase.CreateHandle()
        Alignment = TabAlignment.Left
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        Dim b As New Bitmap(Width, Height)
        Dim g As Graphics = Graphics.FromImage(b)
        g.Clear(Color.Gainsboro)
        For i = 0 To TabCount - 1
            Dim TabRectangle As Rectangle = GetTabRect(i)
            If i = SelectedIndex Then
                g.FillRectangle(Brushes.Cyan, TabRectangle)
            Else
                g.FillRectangle(Brushes.Red, TabRectangle)
            End If
            g.DrawString(TabPages(i).Text, Font, Brushes.White, TabRectangle, New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center})
        Next
        e.Graphics.DrawImage(b.Clone, 0, 0)
        g.Dispose() : b.Dispose()

        MyBase.OnPaint(e)
    End Sub
End Class
