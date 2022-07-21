Public Class WindowAbout

    Private Sub OnClick(sender As Object, e As RoutedEventArgs)
        Try
            System.Diagnostics.Process.Start("http://www.aiut.com.pl")
        Catch ex As Exception

        End Try
    End Sub
End Class
