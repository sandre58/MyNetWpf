// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.Models;
using MyNet.UI.Dialogs.Settings;
using MyNet.Observable;

namespace MyNet.Wpf.Dialogs
{
    public class MessageBoxViewModel : ObservableObject, IMessageBox
    {
        #region Members

        /// <summary>
        /// Gets or sets message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MessageSeverity"/> value that specifies the icon to
        /// display. Default value is <see cref="MessageSeverity.Information"/>.
        /// </summary>
        public MessageSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets title.
        /// </summary>
        public string? Title { get; set; }

        public MessageBoxResultOption Buttons { get; set; }

        public MessageBoxResult DefaultResult { get; set; }

        #endregion Members

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Initialise a new instance of <see cref="MessageBoxViewModel" />.
        /// </summary>
        public MessageBoxViewModel(string message,
                                   string? title = null,
                                   MessageSeverity severity = MessageSeverity.Information,
                                   MessageBoxResultOption buttons = MessageBoxResultOption.OkCancel,
                                   MessageBoxResult defaultResult = MessageBoxResult.OK)
        {
            Message = message;
            Title = title;
            Severity = severity;
            Buttons = buttons;
            DefaultResult = defaultResult;
        }

        #endregion

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj) => obj is MessageBoxViewModel other && Equals(Message, other.Message);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => Message?.GetHashCode() ?? RuntimeHelpers.GetHashCode(this);

        public override string? ToString() => Message;
    }
}
