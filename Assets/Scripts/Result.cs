using System;

public abstract class Result<TResult>
{
    public class Success : Result<TResult>
    {
        public TResult Value { get; }

        public Success(TResult value)
        {
            Value = value;
        }
    }
        
    public class Error : Result<TResult>
    {
        public Exception Message { get; }

        public Error(Exception message)
        {
            Message = message;
        }
    }
}