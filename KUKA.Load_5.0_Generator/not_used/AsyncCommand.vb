Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class AsyncCommand(Of TResult)
    Inherits AsyncCommandBase
    Implements INotifyPropertyChanged
    Private ReadOnly _command As Func(Of Task(Of TResult))
    Private _execution As NotifyTaskCompletion(Of TResult)
    Public Sub New(command As Func(Of Task(Of TResult)))
        _command = command
    End Sub
    Public Overrides Function CanExecute(parameter As Object) As Boolean
        Return True
    End Function
    Public Overrides Function ExecuteAsync(parameter As Object) As Task
        Execution = New NotifyTaskCompletion(Of TResult)(_command())
        Return Execution.TaskCompletion
    End Function

    Public Property Execution() As NotifyTaskCompletion(Of TResult)
        Get
            Return _execution
        End Get
        Private Set(value As NotifyTaskCompletion(Of TResult))
            _execution = value
            NotifyPropertyChanged()
        End Set
    End Property
#Region " Events "
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
#End Region

#Region " Methods "
    ' This method is called by the Set accessor of each property. 
    ' The CallerMemberName attribute that is applied to the optional propertyName 
    ' parameter causes the property name of the caller to be substituted as an argument. 
    Public Sub NotifyPropertyChanged(<CallerMemberName()> Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
#End Region
End Class