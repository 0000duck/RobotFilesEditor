Public Interface IAsyncCommand
    Inherits ICommand
    Function ExecuteAsync(parameter As Object) As Task
End Interface