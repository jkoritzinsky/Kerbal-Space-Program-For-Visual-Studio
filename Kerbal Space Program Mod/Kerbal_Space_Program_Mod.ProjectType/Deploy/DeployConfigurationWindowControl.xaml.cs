//------------------------------------------------------------------------------
// <copyright file="DeployConfigurationWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace KSP4VS.Deploy
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for DeployConfigurationWindowControl.
    /// </summary>
    public partial class DeployConfigurationWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeployConfigurationWindowControl"/> class.
        /// </summary>
        private DeployConfigurationWindowControl()
        {
            this.InitializeComponent();
        }

        internal DeployConfigurationWindowControl(DeployWindowModel model)
            :this()
        {
            DataContext = model;
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "DeployConfigurationWindow");
        }
    }
}