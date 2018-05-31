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

namespace WpfApp2
{
    class BinaryOperations
    {
        public static Func<double, double, double> Additon = ((n1, n2) => n1 + n2);
        public static Func<double, double, double> Subtraction = ((n1, n2) => n1 - n2);
        public static Func<double, double, double> Multiplication = ((n1, n2) => n1 * n2);

        public static double Division(double n1, double n2)
        {
            if (n2 == 0)
            {
                throw new DivideByZeroException();
            }

            return n1 / n2;

        }

        public static double Power(double n1, double n2)
        {
            double _tmp = Math.Pow(n1, n2);
            if (_tmp == double.PositiveInfinity)
            {
                throw new OverflowException();
            }

            return _tmp;
        }

        public static double NthRoot(double n1, double pw)
        {
            if (n1 < 0 && pw % 2 == 0)
            {
                throw new InvalidOperationException();
            }

            return Math.Pow(n1, 1.0 / pw);
        }

    }

    public class AlterLabel : Label
    {
        public AlterLabel() : base() { }

        public event EventHandler ContentChanged;

        protected override void OnContentChanged(object _old_content, object _new_content)
        {
            base.OnContentChanged(_old_content, _new_content);

            if (ContentChanged != null)
                ContentChanged(this, EventArgs.Empty);
        }
    }

    public partial class MainWindow : Window
    {
        public AlterLabel lbl_info = new AlterLabel();

        string decimal_separator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

        bool isAllowOperationFl = false, isCleanlblFl = false;

        Func<double, double, double> operation = null;

        double n;

        public MainWindow()
        {
            InitializeComponent();

            this.lbl_info.Height = 18;
            this.lbl_info.FontSize = 13;
            this.lbl_info.HorizontalContentAlignment = HorizontalAlignment.Right;
            this.lbl_info.VerticalContentAlignment = VerticalAlignment.Center;
            this.lbl_info.Padding = new Thickness(0);
            this.lbl_info.ContentChanged += OnContentChanged;

            stp_screen.Children.Remove(screenTextBox);
            stp_screen.Children.Remove(stp_rbtns);
            stp_screen.Children.Add(lbl_info);
            stp_screen.Children.Add(screenTextBox);
            stp_screen.Children.Add(stp_rbtns);

            foreach (UIElement element in grid.Children)
            {
                Button button = element as Button;
                if (button != null)
                {
                    button.Click += onButtonClick;
                    button.Margin = new Thickness(3);
                    button.FontSize = 20;
                    button.FontFamily = new FontFamily("Consolas");
                    button.FontWeight = FontWeights.Bold;
                    if (button.Content.ToString() == "_decimal_separator")
                    {
                        button.Content = this.decimal_separator;
                    }
                }
            }
        }

        private void onButtonClick(object sender, EventArgs e)
        {
            try
            {
                String _btn_content = ((Button)sender).Content.ToString();

                if (("0123456789" + this.decimal_separator).Contains(_btn_content))
                {
                    AddOperand(_btn_content);
                }

                if (@"+-*/pownth rt".Contains(_btn_content) && this.isAllowOperationFl)
                {
                    AddOperation(_btn_content);
                }

                if (_btn_content == @"+/-" && this.isAllowOperationFl)
                {
                    ChangeSign();
                }

                if (_btn_content == "=" && this.operation != null && this.isAllowOperationFl)
                {
                    Calculate();
                }

                if ("sin(n)cos(n)tg(n)".Contains(_btn_content) && this.isAllowOperationFl)
                {
                    Trigonometric(_btn_content);
                }

                if (_btn_content == "<-" && screenTextBox.Text.Length > 0)
                {
                    CleanEntry(1);
                }

                if (_btn_content == "CE" && screenTextBox.Text.Length > 0)
                {
                    CleanEntry(screenTextBox.Text.Length);
                }

                if (_btn_content == "CA")
                {
                    CleanAll();
                }
            }
            catch (OverflowException)
            {
                CleanAll();
                this.lbl_info.Content = "Overflow Exeption!";
                this.isCleanlblFl = true;
            }

            catch (DivideByZeroException)
            {
                CleanAll();
                this.lbl_info.Content = "Zero division!";
                this.isCleanlblFl = true;
            }

            catch (FormatException)
            {
                CleanAll();
                this.lbl_info.Content = "Invalid Operand!";
                this.isCleanlblFl = true;
            }
            catch (InvalidOperationException)
            {
                CleanAll();
                this.lbl_info.Content = "Invalid Operand!";
                this.isCleanlblFl = true;
            }

        }

        private void OnContentChanged(object sender, EventArgs e)
        {
            if (this.isCleanlblFl)
            {
                ((AlterLabel)sender).Content = screenTextBox.Text;
                this.isCleanlblFl = false;
            }
        }

        private void AddOperand(string content)
        {
            if (content == this.decimal_separator)
            {
                if (screenTextBox.Text.Contains(this.decimal_separator) | !this.isAllowOperationFl)
                {
                    return;
                }

                if (this.isAllowOperationFl)
                {
                    this.isAllowOperationFl = false;
                }

            }
            else
            {
                if (!isAllowOperationFl)
                {
                    this.isAllowOperationFl = true;
                }
            }

            screenTextBox.Text = screenTextBox.Text + content;
            lbl_info.Content = lbl_info.Content + content;

            return;
        }

        private void AddOperation(string content)
        {
            if (this.operation == null)
            {
                this.n = double.Parse(screenTextBox.Text);
            }
            else
            {
                this.n = operation(this.n, double.Parse(screenTextBox.Text));
            }

            CleanEntry(screenTextBox.Text.Length);
            string _info = "";

            switch (content)
            {
                case "+": this.operation = BinaryOperations.Additon; _info = n.ToString() + " + "; break;
                case "-": this.operation = BinaryOperations.Subtraction; _info = n.ToString() + " - "; break;
                case "*": this.operation = BinaryOperations.Multiplication; _info = n.ToString() + " * "; break;
                case @"/": this.operation = BinaryOperations.Division; _info = n.ToString() + @" / "; break;
                case "nth rt": this.operation = BinaryOperations.NthRoot; _info = String.Format("nth root {0}, n: ", n); break;
                case "pow": this.operation = BinaryOperations.Power; _info = String.Format("pow {0}, n: ", n); break;

            }
            lbl_info.Content = _info;

            return;
        }

        private void ChangeSign()
        {
            int _tmp = screenTextBox.Text.Length;
            screenTextBox.Text = (double.Parse(screenTextBox.Text) * -1).ToString();
            lbl_info.Content = lbl_info.Content.ToString().Substring(0, lbl_info.Content.ToString().Length - _tmp) + screenTextBox.Text;

            return;
        }

        private void Calculate()
        {
            this.n = operation(this.n, double.Parse(screenTextBox.Text));
            screenTextBox.Text = this.n.ToString();
            lbl_info.Content = String.Format("{0} = {1}", lbl_info.Content, n);

            this.operation = null;
            this.isCleanlblFl = true;

            return;
        }

        private void Trigonometric(string btn_content)
        {
            string _info = "r";
            double operand = double.Parse(screenTextBox.Text);
            double _tmp = operand;
            if (rbtn_deg.IsChecked == true)
            {
                _tmp = operand * (Math.PI / 180.0);
                _info = "d";
            }
            else if (rbtn_grd.IsChecked == true)
            {
                _tmp = operand * (Math.PI / 200.0);
                _info = "g";
            }

            CleanAll();

            switch (btn_content)
            {
                case "sin(n)": _info = "sin" + _info; _tmp = Math.Sin(_tmp); break;
                case "cos(n)": _info = "cos" + _info; _tmp = Math.Cos(_tmp); break;
                case "tg(n)": _info = "tg" + _info; _tmp = Math.Tan(_tmp); break;
            }
                  
            screenTextBox.Text = _tmp.ToString();
            lbl_info.Content = String.Format("{0}({1}) = {2}", _info, operand, _tmp);

            this.isAllowOperationFl = true;
            this.isCleanlblFl = true;

            return;

        }

        private void CleanEntry(int _length)
        {
            if (screenTextBox.Text.Length == 2 && screenTextBox.Text[0] == '-')
            {
                _length = 2;
            }

            screenTextBox.Text = screenTextBox.Text.Substring(0, screenTextBox.Text.Length - _length);
            lbl_info.Content = lbl_info.Content.ToString().Substring(0, lbl_info.Content.ToString().Length - _length);


            if (screenTextBox.Text.Length == 0 || screenTextBox.Text[screenTextBox.Text.Length - 1].ToString() == this.decimal_separator)
            {
                this.isAllowOperationFl = false;
            }
            else
            {
                this.isAllowOperationFl = true;
            }

            return;
        }

        private void CleanAll()
        {
            screenTextBox.Text = "";
            lbl_info.Content = "";
            this.operation = null;
        }

    }
}
