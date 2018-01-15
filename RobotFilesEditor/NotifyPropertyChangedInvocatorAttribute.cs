using System;

namespace RobotFilesEditor
{
    internal class NotifyPropertyChangedInvocatorAttribute : Attribute
    {        public NotifyPropertyChangedInvocatorAttribute()
        {
        }

        public NotifyPropertyChangedInvocatorAttribute([NotNull] string parameterName)
        {
            ParameterName = parameterName;
        }

        [CanBeNull]
        public string ParameterName { get; private set; }
    }

    internal class NotNullAttribute : Attribute
    {
    }

    internal class CanBeNullAttribute : Attribute
    {
    }
}