using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace wpf_calc
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            text_box_qst.TextChanged += TextChanged;
        }

        private void TextChanged(object sender, RoutedEventArgs e)
        {
            string s = ((TextBox)sender).Text;

            if (CheckSpell((TextBox)sender))
            {
                Expression current_exp = new Expression(s);
                if (current_exp.Parsed)
                {
                    text_box_ans.Text = Convert.ToString(current_exp.Calc()) + "\n" + current_exp.ToString();
                }
                else
                {
                    text_box_ans.Text = "error\n" + current_exp.ToString();
                }
            }
            else
                text_box_ans.Text = "error";
        }

        private bool CheckSpell(TextBox t)
        {
            string s = t.Text;
            if (s == "")
                return false;
            for(int i = 0; i < s.Length; i++)
                if (!(CheckChar(s[i])))
                    return false;
            return true;
        }

        private bool CheckChar(char q)
        {
            if (q >= '0' && q <= '9')
                return true;
            if (q == ' ')
                return true;
            if (q == '(' || q == ')')
                return true;
            if (q == '+' || q == '-' ||
                q == '*' || q == '/')
                return true;
            return false;
                
        }
    }

}
