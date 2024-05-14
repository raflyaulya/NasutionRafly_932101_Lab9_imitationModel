using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet.Numerics.Distributions;

namespace lab9
{
    public partial class Form1 : Form
    {
        private double[] probabilities = new double[5]; // array stores event probabilities
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            InitializeProbabilities();
        }
        private void InitializeProbabilities()
        {
            TextBox[] probabilityTextBoxes = { textBox1, textBox2, textBox3, textBox4, textBox5 };

            // Get the probability from the textbox and initialize it into the array
            for (int i = 0; i < probabilities.Length; i++)
            {
                double probability;
                if (double.TryParse(probabilityTextBoxes[i].Text, out probability))
                {
                    probabilities[i] = probability;
                }
                else
                {
                    // If the content in the text box cannot be parsed, the default probability value is used
                    probabilities[i] = 0.2;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitializeProbabilities();
            if (!ValidateProbabilities())
            {
                MessageBox.Show("Please make sure that the total probability of all events is 1!", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBox6.Text, out int trials) || trials <= 0)
            {
                MessageBox.Show("Please enter a valid number of trials!", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int[] eventCounts = new int[probabilities.Length]; // Store the number of times each event occurs


            for (int i = 0; i < trials; i++)
            {
                double randomNumber = random.NextDouble();

                double cumulativeProbability = 0;
                for (int j = 0; j < probabilities.Length; j++)
                {
                    cumulativeProbability += probabilities[j];
                    if (randomNumber < cumulativeProbability)
                    {
                        eventCounts[j]++;
                        break;
                    }
                }
            }
        

            double[] meanAndVariance = CalculateMeanAndVariance(eventCounts, trials); // Calculate Average and variance
            double chiSquare = CalculateChiSquare(eventCounts, trials); // Calculate chi-square statistic
            double pValue = ChiSquared.InvCDF(eventCounts.Length - 1, 1 - 0.05); // Set significance level to 0.05 and calculate critical value

            UpdateChart(eventCounts, trials);

            MessageBox.Show($"Average: {meanAndVariance[0]}\nVariance: {meanAndVariance[1]}\n" +
                $"Relative Error of Average: {meanAndVariance[2]}\nRelative Error of Variance: {meanAndVariance[3]}\n" +
                $"Chi-Square Statistic: {chiSquare}\nCritical Value: {pValue}\n" +
                $"Chi-Square Test Result: {(chiSquare > pValue ? "chiSquare > pValue is true" : "chiSquare > pValue is false")}", "Results", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private double[] CalculateMeanAndVariance(int[] eventCounts, int trials)
        {
            double[] meanAndVariance = new double[4]; // [Average, Variance, Relative Error of Average, Relative Error of Variance]
            double sum = 0;
            double sumOfSquares = 0;

            // Calculate sum of Average and variance
            for (int i = 0; i < eventCounts.Length; i++)
            {
                sum += eventCounts[i] * probabilities[i];
                sumOfSquares += eventCounts[i] * probabilities[i] * probabilities[i];
            }

            double mean = sum / trials;
            double variance = sumOfSquares / trials - mean * mean;

            // Calculate relative errors
            double relativeErrorMean = Math.Sqrt(variance / trials) / mean;
            double relativeErrorVariance = Math.Sqrt(2.0 / trials) / Math.Sqrt(variance);

            meanAndVariance[0] = mean;
            meanAndVariance[1] = variance;
            meanAndVariance[2] = relativeErrorMean;
            meanAndVariance[3] = relativeErrorVariance;

            return meanAndVariance;
        }
        private double CalculateChiSquare(int[] eventCounts, int trials)
        {
            double chiSquare = 0;
            double[] expectedCounts = new double[eventCounts.Length];

            // Calculate expected counts
            for (int i = 0; i < expectedCounts.Length; i++)
            {
                expectedCounts[i] = probabilities[i] * trials;
            }

            // Calculate chi-square statistic
            for (int i = 0; i < eventCounts.Length; i++)
            {
                chiSquare += Math.Pow(eventCounts[i] - expectedCounts[i], 2) / expectedCounts[i];
            }

            return chiSquare;
        }



        private bool ValidateProbabilities()
        {
            double sum = probabilities.Sum();
            return sum >= 0.999 && sum <= 1.001;
        }

        private void UpdateChart(int[] eventCounts, int trials)
        {
            chart1.Series.Clear();
            chart1.Series.Add("Event Frequency");
            // add data
            for (int i = 0; i < eventCounts.Length; i++)
            {
                double frequency = (double)eventCounts[i] / trials;
                chart1.Series["Event Frequency"].Points.AddXY($"Event {i + 1}", frequency);
            }
        }
    }
}