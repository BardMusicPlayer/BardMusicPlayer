/*
 * NumberBox Control Prototype
 * @author: Kevin Taha 
 * t-ketaha@microsoft.com
 * 
 * C# Prototype for UWP NumberBox (Microsoft XAML Controls Team)
 * 
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Diagnostics;
using Windows.Globalization.NumberFormatting;
using System.Data;
using Windows.Graphics.Printing3D;

namespace NumberBox
{

    // States for Increment and Decrement Buttons, changable by User
    public enum NumberBoxSpinButtonPlacementMode
    {
        Hidden,
        Inline
    };
    

    // Boundary States for min and max modes. Number wrapping only occurs on stepping if WrapEnabled. 
    public enum NumberBoxMinMaxMode
    {
        None,
        MinEnabled,
        MaxEnabled,
        MinAndMaxEnabled,
        WrapEnabled
    }


    // Validation modes, IconMessage and TextBlockMessage are not fully implemented - just yielding red borders. Waiting for Validation PR. 
    public enum NumberBoxBasicValidationMode
    {
        InvalidInputOverwritten,
        IconMessage,
        TextBlockMessage, 
        Disabled
    };

    public enum NumberBoxNumberRounder
    {
        IncrementNumberRounder,
        SignificantDigitsNumberRounder
    }


    public sealed partial class NumberBox : TextBox
    {
       
        /* Value Storage Properties
         * 
         */
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumberBox), new PropertyMetadata((double) 0 ));


        /* Validation properties
         * 
         */
        public NumberBoxBasicValidationMode BasicValidationMode
        {
            get { return (NumberBoxBasicValidationMode)GetValue(BasicValidationModeProperty); }
            set { SetValue(BasicValidationModeProperty, value); }
        }

        public static readonly DependencyProperty BasicValidationModeProperty =
            DependencyProperty.Register("BasicValidationMode", typeof(NumberBoxBasicValidationMode), typeof(NumberBox), new PropertyMetadata(NumberBoxBasicValidationMode.IconMessage));

        public bool HasError
        {
            get { return (bool)GetValue(HasErrorProperty); }
            set { SetValue(HasErrorProperty, value); }
        }
        // Primary Handlers for switching error states (validation)
        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.Register("HasError", typeof(bool), typeof(NumberBox), new PropertyMetadata(false, HasErrorUpdated));



        /* Stepping Properties
         * 
         */
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        public static readonly DependencyProperty MinValueProperty = 
            DependencyProperty.Register( "MinValue", typeof(double), typeof(NumberBox), new PropertyMetadata( (double) 0) );

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register( "MaxValue", typeof(double), typeof(NumberBox), new PropertyMetadata( (double) 0) );

        public double StepFrequency
        {
            get { return (double) GetValue(StepFrequencyProperty); }
            set { SetValue(StepFrequencyProperty, value); }
        }
        public static readonly DependencyProperty StepFrequencyProperty =
            DependencyProperty.Register("StepFrequency", typeof(double), typeof(NumberBox), new PropertyMetadata( (double) 1));
        

        public NumberBoxSpinButtonPlacementMode SpinButtonPlacementMode
        {
            get { return (NumberBoxSpinButtonPlacementMode)GetValue(SpinButtonPlacementModeProperty); }
            set { SetValue(SpinButtonPlacementModeProperty, value); }
        }
        public static readonly DependencyProperty SpinButtonPlacementModeProperty =
            DependencyProperty.Register("UpDownPlacementMode", typeof(NumberBoxSpinButtonPlacementMode), typeof(NumberBox), new PropertyMetadata(NumberBoxSpinButtonPlacementMode.Hidden, HasSpinnerUpdated ));


        public bool HyperScrollEnabled
        {
            get { return (bool)GetValue(HyperScrollEnabledProperty); }
            set { SetValue(HyperScrollEnabledProperty, value); }
        }
        public static readonly DependencyProperty HyperScrollEnabledProperty =
            DependencyProperty.Register("HyperScrollEnabled", typeof(bool), typeof(NumberBox), new PropertyMetadata(false));

        public NumberBoxMinMaxMode MinMaxMode
        {
            get { return (NumberBoxMinMaxMode)GetValue(MinMaxModeProperty); }
            set { SetValue(MinMaxModeProperty, value); }
        }
        public static readonly DependencyProperty MinMaxModeProperty =
            DependencyProperty.Register( "MinMaxMode", typeof(NumberBoxMinMaxMode), typeof(NumberBox), new PropertyMetadata(NumberBoxMinMaxMode.None) );



        /* Precision Properties
         * 
         */

        private DecimalFormatter Formatter { get; set; }
        public int FractionDigits
        {
            get { return (int)GetValue(FractionDigitsProperty); }
            set { SetValue(FractionDigitsProperty, value); }
        }
        public static readonly DependencyProperty FractionDigitsProperty =
            DependencyProperty.Register("FractionDigits", typeof(int), typeof(NumberBox), new PropertyMetadata( (new DecimalFormatter()).FractionDigits, HasFormatterUpdated));

        public int IntegerDigits
        {
            get { return (int)GetValue(IntegerDigitsProperty); }
            set { SetValue(IntegerDigitsProperty, value); }
        }
        public static readonly DependencyProperty IntegerDigitsProperty =
            DependencyProperty.Register("IntegerDigits", typeof(int), typeof(NumberBox), new PropertyMetadata( (new DecimalFormatter()).IntegerDigits, HasFormatterUpdated) );


        public int SignificantDigits
        {
            get { return (int)GetValue(SignificantDigitsProperty); }
            set { SetValue(SignificantDigitsProperty, value); }
        }
        public static readonly DependencyProperty SignificantDigitsProperty =
            DependencyProperty.Register("SignificantDigits", typeof(int), typeof(NumberBox), new PropertyMetadata((new DecimalFormatter()).SignificantDigits, HasFormatterUpdated));


        public bool IsDecimalPointAlwaysDisplayed
        {
            get { return (bool)GetValue(IsDecimalPointAlwaysDisplayedProperty); }
            set { SetValue(IsDecimalPointAlwaysDisplayedProperty, value); }
        }

        public static readonly DependencyProperty IsDecimalPointAlwaysDisplayedProperty =
            DependencyProperty.Register("IsDecimalPointAlwaysDisplayed", typeof(bool), typeof(NumberBox), new PropertyMetadata(false, HasFormatterUpdated));

        public bool IsZeroSigned
        {
            get { return (bool)GetValue(IsZeroSignedProperty); }
            set { SetValue(IsZeroSignedProperty, value); }
        }

        public static readonly DependencyProperty IsZeroSignedProperty =
            DependencyProperty.Register("IsZeroSigned", typeof(bool), typeof(NumberBox), new PropertyMetadata(false, HasFormatterUpdated));



        /* Rounding Properties
         * 
         */

        public RoundingAlgorithm RoundingAlgorithm
        {
            get { return (RoundingAlgorithm)GetValue(RoundingAlgorithmProperty); }
            set { SetValue(RoundingAlgorithmProperty, value); }
        }

        public static readonly DependencyProperty RoundingAlgorithmProperty =
            DependencyProperty.Register("RoundingAlgorithm", typeof(RoundingAlgorithm), typeof(NumberBox), new PropertyMetadata(RoundingAlgorithm.None, HasFormatterUpdated));

        public NumberBoxNumberRounder NumberRounder
        {
            get { return (NumberBoxNumberRounder)GetValue(NumberRounderProperty); }
            set { SetValue(NumberRounderProperty, value); }
        }
        public static readonly DependencyProperty NumberRounderProperty =
            DependencyProperty.Register("NumberRounder", typeof(NumberBoxNumberRounder), typeof(NumberBox), new PropertyMetadata(NumberBoxNumberRounder.IncrementNumberRounder, HasFormatterUpdated));


        public double IncrementPrecision
        {
            get { return (double)GetValue(IncrementPrecisionProperty); }
            set { SetValue(IncrementPrecisionProperty, value); }
        }
        public static readonly DependencyProperty IncrementPrecisionProperty =
            DependencyProperty.Register("IncrementPrecision", typeof(double), typeof(NumberBox), new PropertyMetadata( (double) 1, HasFormatterUpdated));


        public uint SignificantDigitPrecision
        {
            get { return (uint)GetValue(SignificantDigitPrecisionProperty); }
            set { SetValue(SignificantDigitPrecisionProperty, value); }
        }
        public static readonly DependencyProperty SignificantDigitPrecisionProperty =
            DependencyProperty.Register("SignificantDigitPrecision", typeof(uint), typeof(NumberBox), new PropertyMetadata(1, HasFormatterUpdated));





        /* Calculation Properties
         * 
         */
        public bool AcceptsCalculation
        {
            get { return (bool)GetValue(AcceptsCalculationProperty); }
            set { SetValue(AcceptsCalculationProperty, value); }
        }
        public static readonly DependencyProperty AcceptsCalculationProperty =
            DependencyProperty.Register("AcceptsCalculation", typeof(bool), typeof(NumberBox), new PropertyMetadata(false));






        public NumberBox()
        {
            this.DefaultStyleKey = typeof(TextBox);
            this.LostFocus += new RoutedEventHandler(ValidateInput);
            this.PointerExited += new PointerEventHandler(RefreshErrorState);
            this.KeyUp += new KeyEventHandler( KeyPressed );
            this.Formatter = new DecimalFormatter();

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // enable spinner buttons and hyperscroll
            SetSpinnerButtonsState(this.SpinButtonPlacementMode);
            this.PointerWheelChanged += new PointerEventHandler(OnScroll);

            // Handles default values set for Text or Value
            InitiateFormatter();

            if (this.Text != null && this.Text != "")
            {
                ValidateInput(this, new RoutedEventArgs());
            }
            if (this.Value != 0)
            {
                this.Text = Formatter.FormatDouble(Value);
            }


        }

        // Handlers for spin button visibility and event handlers
        void SetSpinnerButtonsState( NumberBoxSpinButtonPlacementMode state )
        {

            DependencyObject DownSpinButton = this.GetTemplateChild("DownSpinButton");
            DependencyObject UpSpinButton = this.GetTemplateChild("UpSpinButton");
            ( (Button)DownSpinButton ).Click += new RoutedEventHandler( OnDownClick );
            ( (Button)UpSpinButton).Click += new RoutedEventHandler(OnUpClick);

            if ( state == NumberBoxSpinButtonPlacementMode.Inline )
            {
                VisualStateManager.GoToState(this, "SpinButtonsVisible", false);
            }


        }

        private static void HasSpinnerUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numBox = d as NumberBox;
            if ( numBox != null && numBox.SpinButtonPlacementMode == NumberBoxSpinButtonPlacementMode.Inline )
            {
                VisualStateManager.GoToState(numBox, "SpinButtonsVisible", false);
            }
            else if ( numBox != null && numBox.SpinButtonPlacementMode == NumberBoxSpinButtonPlacementMode.Hidden )
            {
                VisualStateManager.GoToState(numBox, "SpinButtonsCollapsed", false);
            }
        }

        // Event handlers for spin button clicks
        void OnDownClick(object sender, RoutedEventArgs e)
        {
            StepValue(false);
        }

        void OnUpClick(object sender, RoutedEventArgs e)
        {
            StepValue(true);
        }

        void OnScroll(object sender, PointerRoutedEventArgs e)
        {
            if (!HyperScrollEnabled)
            {
                return;
            }
            int delta = e.GetCurrentPoint(this).Properties.MouseWheelDelta;
            if ( delta > 0 )
            {
                StepValue(true);
            }
            else if ( delta < 0 )
            {
                StepValue(false);
            }

        }

        void KeyPressed(object sender, KeyRoutedEventArgs e)
        {
            // https://docs.microsoft.com/en-us/uwp/api/Windows.System.VirtualKey
            switch ( (int) e.Key)
            {
                // Keyboard Up Key
                case 204:
                case 38:
                    StepValue(true);
                    break;
                // Keyboard Down Key
                case 40:
                    StepValue(false);
                    break;

            }
        }


        // Steps value by user set increment.
        void StepValue( bool sign )
        {
            // Validate input before stepping, this includes evaluation of calculation
            ValidateInput( this, new RoutedEventArgs() );

            if (sign)
            {
                Value += StepFrequency;
            }
            else
            {
                Value -= StepFrequency;
            }

            // Wrap value on step if applies
            if ( MinMaxMode == NumberBoxMinMaxMode.WrapEnabled && IsOutOfBounds(Value) )
            {
                while ( Value > MaxValue )
                {
                    Value = MinValue + (Value - MaxValue) - 1;
                }
                while ( Value < MinValue )
                {
                    Value = MaxValue - Math.Abs(Value - MinValue) + 1;
                }
            }
            this.Text = Value.ToString();

            ProcessInput(Value);
        }


        // Uses DecimalFormatter to validate that input is compliant
        void ValidateInput(object sender, RoutedEventArgs e)
        {
            bool isEval = false;
            // Handles case for empty textbox, should remain valid and treated by value as 0 input for now
            if (this.Text == "" && !IsOutOfBounds(0) )
            {
                Value = 0;
                SetErrorState(false);
                return;
            }


            if ( BasicValidationMode == NumberBoxBasicValidationMode.Disabled )
            {
                return;
            }

            if ( AcceptsCalculation )
            {
                EvaluateInput();
                isEval = true;
            }

            DecimalFormatter df = this.Formatter;
            Nullable<double> parsedNum = df.ParseDouble(this.Text);

            // Give Validaton error if no match 
            if ( parsedNum == null || IsOutOfBounds( (double) parsedNum) )
            {
                // Overwrite with last  value when invalid value is parsed
                if ( BasicValidationMode == NumberBoxBasicValidationMode.InvalidInputOverwritten && !isEval )
                {
                    SetErrorState(false);
                    ProcessInput(Value);
                    return;
                }

                SetErrorState(true);

            }
            else
            {
                // Set Valid state and start input processing
                SetErrorState(false);
                ProcessInput( (double) parsedNum);
            }
        }

        bool IsOutOfBounds(double parsedNum)
        {
            switch( this.MinMaxMode )
            {
                case NumberBoxMinMaxMode.None:
                    return false;

                case NumberBoxMinMaxMode.MinAndMaxEnabled:
                    if ( parsedNum < this.MinValue || parsedNum > this.MaxValue )
                    {
                        return true;
                    }
                    break;

                case NumberBoxMinMaxMode.MinEnabled:
                    if ( parsedNum < this.MinValue )
                    {
                        return true;
                    }
                    break;
                case NumberBoxMinMaxMode.MaxEnabled:
                    if ( parsedNum > this.MaxValue )
                    {
                        return true;
                    }
                    break;
                case NumberBoxMinMaxMode.WrapEnabled:
                    if (parsedNum < this.MinValue || parsedNum > this.MaxValue)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        // Performs Calculator Operations
        void EvaluateInput()
        {
            if (this.Text.Equals("-0") && IsZeroSigned)
            {
                return;
            }

            String result;
            DataTable dt = new DataTable();
            try
            {
                result = Convert.ToString(dt.Compute(this.Text, null));
            }
            catch (Exception e)
            {
                return;
            } 
            this.Text = result;
        }



        // Master function for handling all other input processing and precision settings
        void ProcessInput(double val)
        {
            this.Text = Formatter.Format(val);
            this.Value = (double) Formatter.ParseDouble(this.Text);
        }


        private void InitiateFormatter()
        {
            DecimalFormatter df = this.Formatter;
            if ( df.FractionDigits != this.FractionDigits )
                df.FractionDigits = this.FractionDigits;

            if ( df.IntegerDigits != this.IntegerDigits )
                df.IntegerDigits = this.IntegerDigits;

            if ( df.SignificantDigits != this.SignificantDigits )
                df.SignificantDigits = this.SignificantDigits;

            if ( df.IsZeroSigned != this.IsZeroSigned )
                df.IsZeroSigned = this.IsZeroSigned;

            if ( df.IsDecimalPointAlwaysDisplayed != this.IsDecimalPointAlwaysDisplayed )
                df.IsDecimalPointAlwaysDisplayed = this.IsDecimalPointAlwaysDisplayed;

            // Set Rounding algorithm. For some reason this throws an inexplicable invalid parameter exception so using try-catch for now
            setRounder();


        }

        // Reconstructs formatter if settings for it have changed
        private static void HasFormatterUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numBox = d as NumberBox;
            numBox.InitiateFormatter();
        }

        // Executed on change of HasError Property
        private static void HasErrorUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numBox = d as NumberBox;
            if (numBox != null)
            {
                if (numBox.HasError)
                {
                    VisualStateManager.GoToState(numBox, "Invalid", false);
                }
                else
                {
                    VisualStateManager.GoToState(numBox, "Normal", false);
                }
            }
        }

        // Sets the Error State of the TextBox. 
        void SetErrorState(bool error)
        {
            if( error )
            {
                if ( this.HasError )
                {
                    VisualStateManager.GoToState(this, "Invalid", false);

                }
                else
                {
                    this.HasError = true;
                }
            }
            else
            {
                this.HasError = false;
            }
        }

        // Ensures that invalid state persists when pointer exits, rather than resetting to normal state.
        void RefreshErrorState(object sender, RoutedEventArgs e)
        {
            NumberBox numBox = (NumberBox) sender;

            if ( numBox.HasError )
            {
                VisualStateManager.GoToState(this, "Invalid", false);
            }
        }


        void setRounder()
        {
            // Prevents an exception from being thrown
            if (RoundingAlgorithm == RoundingAlgorithm.None )
            {
                Formatter.NumberRounder = null;
                return;
            }


            if ( this.NumberRounder == NumberBoxNumberRounder.IncrementNumberRounder )
            {
                IncrementNumberRounder nr = new IncrementNumberRounder();
                if (this.IncrementPrecision != 0 )
                    nr.Increment = this.IncrementPrecision;
                nr.RoundingAlgorithm = this.RoundingAlgorithm;
                Formatter.NumberRounder = nr;
            }
            else
            {
                SignificantDigitsNumberRounder nr = new SignificantDigitsNumberRounder();
                if ( this.SignificantDigitPrecision != 0)
                    nr.SignificantDigits = this.SignificantDigitPrecision;
                nr.RoundingAlgorithm = this.RoundingAlgorithm;
                Formatter.NumberRounder = nr;

            }

        }

    }
}
