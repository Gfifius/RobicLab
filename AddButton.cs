using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace RL
{
    class AddButton
    {
        public string Content { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public ICommand Command { get; set; }

        public AddButton(string content, int row = 0, int column = 0, ICommand command = null)
        {
            Content = content;
            Row = row;
            Column = column;
            Command = command;

            Button newbutt = new Button();
            newbutt.Content = "lkds;dklfsd";
            newbutt.Height = 50;
            newbutt.Width = 200;
            //newbutt.Margin = new Thickness(700, 500, 0, 0);
            //newbutt.Click += Button_Click;
            //newbutt.Name = "buut";
            //stackForButton.Children.Add(newbutt);
        }
    }
}
