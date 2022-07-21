Public MustInherit Class AsyncCommandBase
    Implements IAsyncCommand

    Public MustOverride Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
    Public MustOverride Function ExecuteAsync(parameter As Object) As Task Implements IAsyncCommand.ExecuteAsync
    Public Async Sub Execute(parameter As Object) Implements ICommand.Execute
        Await ExecuteAsync(parameter)
    End Sub
    Public Custom Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
        AddHandler(ByVal value As EventHandler)
            AddHandler CommandManager.RequerySuggested, value
        End AddHandler
        RemoveHandler(ByVal value As EventHandler)
            RemoveHandler CommandManager.RequerySuggested, value
        End RemoveHandler
        RaiseEvent()
            CommandManager.InvalidateRequerySuggested()
        End RaiseEvent
    End Event
End Class