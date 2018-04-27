﻿using libsignalservice;
using Microsoft.Extensions.Logging;
using Signal_Windows.Lib;
using Signal_Windows.Models;
using Signal_Windows.ViewModels;
using Signal_Windows.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Signal_Windows
{
    public class SignalWindowsFrontend : ISignalFrontend
    {
        private readonly ILogger Logger = LibsignalLogging.CreateLogger<SignalWindowsFrontend>();
        public CoreDispatcher Dispatcher { get; }
        public ViewModelLocator Locator { get; }
        public int ViewId { get; }
        public SignalWindowsFrontend(CoreDispatcher dispatcher, ViewModelLocator locator, int viewId)
        {
            Dispatcher = dispatcher;
            Locator = locator;
            ViewId = viewId;
        }
        public void AddOrUpdateConversation(SignalConversation conversation, SignalMessage updateMessage)
        {
            Locator.MainPageInstance.AddOrUpdateConversation(conversation, updateMessage);
        }

        public void HandleIdentitykeyChange(LinkedList<SignalMessage> messages)
        {
            Locator.MainPageInstance.HandleIdentitykeyChange(messages);
        }

        public AppendResult HandleMessage(SignalMessage message, SignalConversation conversation)
        {
            return Locator.MainPageInstance.HandleMessage(message, conversation);
        }

        public void HandleMessageUpdate(SignalMessage updatedMessage)
        {
            Locator.MainPageInstance.HandleMessageUpdate(updatedMessage);
        }

        public void ReplaceConversationList(List<SignalConversation> conversations)
        {
            Locator.MainPageInstance.ReplaceConversationList(conversations);
        }

        public void HandleAuthFailure()
        {
            Logger.LogInformation("HandleAuthFailure() {0}", ViewId);
            if (ViewId == App.MainViewId)
            {
                Frame f = (Frame)Window.Current.Content;
                f.Navigate(typeof(StartPage));
                CoreApplication.GetCurrentView().CoreWindow.Activate();
                ApplicationViewSwitcher.TryShowAsStandaloneAsync(App.MainViewId);
            }
            else
            {
                CoreApplication.GetCurrentView().CoreWindow.Close();
            }
        }

        public void HandleAttachmentStatusChanged(SignalAttachment sa)
        {
            Locator.MainPageInstance.HandleAttachmentStatusChanged(sa);
        }

        public void HandleMessageRead(long messageIndex, SignalConversation conversation)
        {
            Locator.MainPageInstance.HandleMessageRead(messageIndex, conversation);
        }

        public void HandleUnreadMessage(SignalMessage message)
        {
            if (ApplicationView.GetForCurrentView().Id == App.MainViewId)
            {
                if (message.Author != null)
                {
                    SignalNotifications.TryVibrate(true);
                    SignalNotifications.SendMessageNotification(message);
                    SignalNotifications.SendTileNotification(message);
                }
            }
        }
    }
}
