Imports System.ComponentModel
Imports System.Threading.Tasks
Imports System.Runtime.CompilerServices

Public NotInheritable Class NotifyTaskCompletion(Of TResult)
    Implements INotifyPropertyChanged

#Region " Fields "
    Private _Task As Task(Of TResult)
    Private _TaskCompletion As Task
#End Region

#Region " Events "
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
#End Region

#Region " Constructors "
    Public Sub New(task As Task(Of TResult))
        _Task = task
        If Not task.IsCompleted Then
            _TaskCompletion = WatchTaskAsync(task)
        End If
    End Sub
#End Region

#Region " Properties "
    Public Property TaskCompletion() As Task
        Get
            Return _TaskCompletion
        End Get
        Private Set(value As Task)
            _TaskCompletion = value
        End Set
    End Property
    Public Property Task() As Task(Of TResult)
        Get
            Return _Task
        End Get
        Private Set(value As Task(Of TResult))
            _Task = value
        End Set
    End Property
    Public ReadOnly Property Result() As TResult
        Get
            Return If((_Task.Status = TaskStatus.RanToCompletion), _Task.Result, Nothing)
        End Get
    End Property
    Public ReadOnly Property Status() As TaskStatus
        Get
            Return _Task.Status
        End Get
    End Property
    Public ReadOnly Property IsCompleted() As Boolean
        Get
            Return _Task.IsCompleted
        End Get
    End Property
    Public ReadOnly Property IsNotCompleted() As Boolean
        Get
            Return Not _Task.IsCompleted
        End Get
    End Property
    Public ReadOnly Property IsSuccessfullyCompleted() As Boolean
        Get
            Return _Task.Status = TaskStatus.RanToCompletion
        End Get
    End Property
    Public ReadOnly Property IsCanceled() As Boolean
        Get
            Return _Task.IsCanceled
        End Get
    End Property
    Public ReadOnly Property IsFaulted() As Boolean
        Get
            Return _Task.IsFaulted
        End Get
    End Property
    Public ReadOnly Property Exception() As AggregateException
        Get
            Return _Task.Exception
        End Get
    End Property
    Public ReadOnly Property InnerException() As Exception
        Get
            Return If((_Task.Exception Is Nothing), Nothing, _Task.Exception.InnerException)
        End Get
    End Property
    Public ReadOnly Property ErrorMessage() As String
        Get
            Return If((InnerException Is Nothing), Nothing, InnerException.Message)
        End Get
    End Property
#End Region

#Region " Methods "
    ' This method is called by the Set accessor of each property. 
    ' The CallerMemberName attribute that is applied to the optional propertyName 
    ' parameter causes the property name of the caller to be substituted as an argument. 
    Public Sub NotifyPropertyChanged(<CallerMemberName()> Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Private Async Function WatchTaskAsync(task As Task) As Task
        Try
            Await task
        Catch
        End Try
        NotifyPropertyChanged("Status")
        NotifyPropertyChanged("IsCompleted")
        NotifyPropertyChanged("IsNotCompleted")
        If task.IsCanceled Then
            NotifyPropertyChanged("IsCanceled")
        ElseIf task.IsFaulted Then
            NotifyPropertyChanged("IsFaulted")
            NotifyPropertyChanged("Exception")
            NotifyPropertyChanged("InnerException")
            NotifyPropertyChanged("ErrorMessage")
        Else
            NotifyPropertyChanged("IsSuccessfullyCompleted")
            NotifyPropertyChanged("Result")
        End If
    End Function
#End Region
End Class