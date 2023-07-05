using System;
using UnityEngine;

namespace APP
{
    public static class Messager
    {          
        public static Message Send(bool debug, Message message)
        {
            Send(debug, message.Sender, message.Text, message.LogFormat);
            return message;
        }
            
        public static Message Send(bool debug, object sender, string text, LogFormat logFormat = LogFormat.None)
        {
            var message = $"{sender.GetName()}: {text}";

            if (debug)
            {
                switch (logFormat)
                {
                    case LogFormat.Warning:
                        Debug.LogWarning(message);
                        break;
                 
                    case LogFormat.Error:
                        Debug.LogError(message);
                        break;

                    default:
                        Debug.Log(message);
                        break;
                }
            }

            return new Message(sender, message, logFormat);
        }
    }

    
    public struct Message: IMessage
    {
        public Message(string text)
        {
            Sender = null;
            Text = text;
            LogFormat = LogFormat.None;
        }
        
        public Message(string text, LogFormat logFormat)
        {
            Sender = null;
            Text = text;
            LogFormat = logFormat;

        }
        
        public Message(object sender, string text)
        {
            Sender = sender;
            Text = text;
            LogFormat = LogFormat.None;
        }
        
        public Message(object sender, string text, LogFormat logFormat)
        {
            Sender = sender;
            Text = text;
            LogFormat = logFormat;
        }

        public object Sender {get; private set; }
        public string Text {get; private set; }
        public LogFormat LogFormat {get; private set; }   
    
    }

    public enum LogFormat
    {
        None,
        Warning,
        Error
    }


    public interface IMessage
    {
        object Sender { get; }
        string Text { get; }
        LogFormat LogFormat { get; } 
    }
    
    
    public interface IMessager
    {
        event Action<IMessage> Message;
        
        // SEND //
        IMessage Send(string text, LogFormat logFormat = LogFormat.None);
    }
}