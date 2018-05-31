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

        string _decimal_separator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        bool _allow_operation_fl = false, _clean_lbl_fl = false;
        Func<double, double, double> _operation = null;
        double _n;

        public MainWindow()
        {
            InitializeComponent();

            this.lbl_info.Height = 18;
            this.lbl_info.FontSize = 13;
            this.lbl_info.HorizontalContentAlignment = HorizontalAlignment.Right;
            this.lbl_info.VerticalContentAlignment = VerticalAlignment.Center;
            this.lbl_info.Padding = new Thickness(0);
            this.lbl_info.ContentChanged += OnContentChanged;

            stp_screen.Children.Remove(txtbox_screen);
            stp_screen.Children.Remove(stp_rbtns);
            stp_screen.Children.Add(lbl_info);
            stp_screen.Children.Add(txtbox_screen);
            stp_screen.Children.Add(stp_rbtns);

            foreach (UIElement _ch in m_grid.Children)
            {
                Button _btn = _ch as Button;
                if (_btn != null)
                {
                    _btn.Click += btn_Click;
                    _btn.Margin = new Thickness(3);
                    _btn.FontSize = 20;
                    _btn.FontFamily = new FontFamily("Consolas");
                    _btn.FontWeight = FontWeights.Bold;
                    if (_btn.Content.ToString() == "_decimal_separator")
                    {
                        _btn.Content = this._decimal_separator;
                    }
                }
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            try
            {
                String _btn_content = ((Button)sender).Content.ToString();

                if (("0123456789" + this._decimal_separator).Contains(_btn_content))
                {
                    AddOperand(_btn_content);
                }

                if (@"+-*/pownth rt".Contains(_btn_content) && this._allow_operation_fl)
                {
                    AddOperation(_btn_content);
                }

                if (_btn_content == @"+/-" && this._allow_operation_fl)
                {
                    ChangeSign();
                }

                if (_btn_content == "=" && this._operation != null && this._allow_operation_fl)
                {
                    Calculate();
                }

                if ("sin(n)cos(n)tg(n)".Contains(_btn_content) && this._allow_operation_fl)
                {
                    Trigonometric(_btn_content);
                }

                if (_btn_content == "<-" && txtbox_screen.Text.Length > 0)
                {
                    CleanEntry(1);
                }

                if (_btn_content == "CE" && txtbox_screen.Text.Length > 0)
                {
                    CleanEntry(txtbox_screen.Text.Length);
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
                this._clean_lbl_fl = true;
            }

            catch (DivideByZeroException)
            {
                CleanAll();
                this.lbl_info.Content = "Zero division!";
                this._clean_lbl_fl = true;
            }

            catch (FormatException)
            {
                CleanAll();
                this.lbl_info.Content = "Invalid Operand!";
                this._clean_lbl_fl = true;
            }
            catch (InvalidOperationException)
            {
                CleanAll();
                this.lbl_info.Content = "Invalid Operand!";
                this._clean_lbl_fl = true;
            }

        }

        private void OnContentChanged(object sender, EventArgs e)
        {
            if (this._clean_lbl_fl)
            {
                ((AlterLabel)sender).Content = txtbox_screen.Text;
                this._clean_lbl_fl = false;
            }
        }

        private void AddOperand(string btn_content)
        {
            if (btn_content == this._decimal_separator)
            {
                if (txtbox_screen.Text.Contains(this._decimal_separator) | !this._allow_operation_fl)
                {
                    return;
                }

                if (this._allow_operation_fl)
                {
                    this._allow_operation_fl = false;
                }

            }
            else
            {
                if (!_allow_operation_fl)
                {
                    this._allow_operation_fl = true;
                }
            }

            txtbox_screen.Text = txtbox_screen.Text + btn_content;
            lbl_info.Content = lbl_info.Content + btn_content;

            return;
        }

        private void AddOperation(string btn_content)
        {
            if (this._operation == null)
            {
                this._n = double.Parse(txtbox_screen.Text);
            }
            else
            {
                this._n = _operation(this._n, double.Parse(txtbox_screen.Text));
            }

            CleanEntry(txtbox_screen.Text.Length);
            string _info = "";

            switch (btn_content)
            {
                case "+": this._operation = BinaryOperations.Additon; _info = _n.ToString() + " + "; break;
                case "-": this._operation = BinaryOperations.Subtraction; _info = _n.ToString() + " - "; break;
                case "*": this._operation = BinaryOperations.Multiplication; _info = _n.ToString() + " * "; break;
                case @"/": this._operation = BinaryOperations.Division; _info = _n.ToString() + @" / "; break;
                case "nth rt": this._operation = BinaryOperations.NthRoot; _info = String.Format("nth root {0}, n: ", _n); break;
                case "pow": this._operation = BinaryOperations.Power; _info = String.Format("pow {0}, n: ", _n); break;

            }
            lbl_info.Content = _info;

            return;
        }

        private void ChangeSign()
        {
            int _tmp = txtbox_screen.Text.Length;
            txtbox_screen.Text = (double.Parse(txtbox_screen.Text) * -1).ToString();
            lbl_info.Content = lbl_info.Content.ToString().Substring(0, lbl_info.Content.ToString().Length - _tmp) + txtbox_screen.Text;

            return;
        }

        private void Calculate()
        {
            this._n = _operation(this._n, double.Parse(txtbox_screen.Text));
            txtbox_screen.Text = this._n.ToString();
            lbl_info.Content = String.Format("{0} = {1}", lbl_info.Content, _n);

            this._operation = null;
            this._clean_lbl_fl = true;

            return;
        }

        private void Trigonometric(string btn_content)
        {
            string _info = "r";
            double operand = double.Parse(txtbox_screen.Text);
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

            //if n  == NaN;

            txtbox_screen.Text = _tmp.ToString();
            lbl_info.Content = String.Format("{0}({1}) = {2}", _info, operand, _tmp);

            this._allow_operation_fl = true;
            this._clean_lbl_fl = true;

            return;

        }

        private void CleanEntry(int _length)
        {
            if (txtbox_screen.Text.Length == 2 && txtbox_screen.Text[0] == '-')
            {
                _length = 2;
            }

            txtbox_screen.Text = txtbox_screen.Text.Substring(0, txtbox_screen.Text.Length - _length);
            lbl_info.Content = lbl_info.Content.ToString().Substring(0, lbl_info.Content.ToString().Length - _length);


            if (txtbox_screen.Text.Length == 0 || txtbox_screen.Text[txtbox_screen.Text.Length - 1].ToString() == this._decimal_separator)
            {
                this._allow_operation_fl = false;
            }
            else
            {
                this._allow_operation_fl = true;
            }

            return;
        }

        private void CleanAll()
        {
            txtbox_screen.Text = "";
            lbl_info.Content = "";
            this._operation = null;
        }

    }
}
