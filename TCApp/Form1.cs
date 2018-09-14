using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace TCApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            AppendText(this.richTextBox3, Color.Green, "Unchanged text is green. ");
            AppendText(this.richTextBox3, Color.Orange, "Added/Edited text is orange. ");
            AppendText(this.richTextBox3, Color.Red, "Deleted text is red.");
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork +=
                new DoWorkEventHandler(backgroundWorker1_DoWork);
          /*  backgroundWorker2.DoWork +=
                new DoWorkEventHandler(backgroundWorker2_DoWork);
            backgroundWorker1.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
                    backgroundWorker1_RunWorkerCompleted);*/

        }

        private int Compare(List<string[]> list, BackgroundWorker backgroundWorker1, DoWorkEventArgs e)
        {
            string[] linesOfOrigin = list[0];
            string[] linesToCheck = list[1];
            List<Colorator> finalList = new List<Colorator>();
            int[] removed_indexes = new int[linesOfOrigin.Length];
            int totalSize = linesToCheck.Length;
            int cycleCounter = 0;                                            // count cycle for precise position of deleted line
            int[] removed_position = new int[linesOfOrigin.Length];

            for (int i = 0; i < linesOfOrigin.Length; i++)              // initialize array of removed original indexes
            {
                removed_indexes[i] = 1;
                removed_position[i] = 0;
            }

            for (int i = 0; i < linesToCheck.Length; i++)
            {
                progressBar1.Invoke(() => 
                    { progressBar1.Value = 100 * i / linesToCheck.Length; });
                bool found = false;
                for (int j = 0; j < linesOfOrigin.Length; j++)
                {
                    if ((linesToCheck[i] == linesOfOrigin[j]) & (!found) && (removed_indexes[j] != 0))
                    {
                        Colorator colorator = new Colorator(linesOfOrigin[j], Color.Green);
                        finalList.Add(colorator);                     // insert colored text into list
                        removed_indexes[j] = 0;
                        found = true;
                        cycleCounter++;
                        break;
                    }
                }

                if (!found)
                {
                    string[] words = linesToCheck[i].Split(new[] { " ", ",", ";", "." },
                        StringSplitOptions.RemoveEmptyEntries
                        );

                    for (int j = 0; j < linesOfOrigin.Length; j++)
                    {
                        if ((words.Any(linesOfOrigin[j].Contains)) && (removed_indexes[j] != 0))
                        {
                            Colorator colorator = new Colorator(linesOfOrigin[j], Color.Red);
                            Colorator colorator2 = new Colorator(linesToCheck[i], Color.Orange);

                            finalList.Add(colorator);
                            finalList.Add(colorator2);

                            removed_indexes[j] = 0;
                            found = true;
                            cycleCounter++;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    Colorator colorator = new Colorator(linesToCheck[i], Color.Orange);
                    finalList.Add(colorator);
                    cycleCounter++;
                }
                
            }
            // insert colored text into list
            for (int i = 0; i < linesOfOrigin.Length; i++)
            {
                if (removed_indexes[i] == 1)
                {
                    Colorator colorator = new Colorator(linesOfOrigin[i], Color.Red);
                    finalList.Insert(i, colorator);
                }
            }
            for (int i = 0; i < finalList.Count(); i++)
                richTextBox2.Invoke(() => { AppendText(richTextBox2, finalList[i].color, finalList[i].text + "\n"); });
            backgroundWorker1_RunWorkerCompleted();
            return 0;
        }

       /* private int WriteToTextBox(string filename, BackgroundWorker backgroundWorker2, DoWorkEventArgs e)
        {
            string readFile = File.ReadAllText(filename);
            richTextBox1.Invoke(() => { richTextBox1.Text = readFile; });
            textBox1.Invoke(() => { textBox1.Text = filename; });
            backgroundWorker2.CancelAsync();
            return 0;
        } */

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1_RunWorkerCompleted();
            backgroundWorker1.CancelAsync();
            progressBar1.Value = 0;
            openFileDialog1.ShowDialog();
            string filename = openFileDialog1.FileName;
            string readFile = File.ReadAllText(filename);
            richTextBox1.Text = readFile;
            textBox1.Text = filename;
            richTextBox2.Invoke(() => { richTextBox2.Text = ""; });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            backgroundWorker1_RunWorkerCompleted();
            backgroundWorker1.CancelAsync();
            progressBar1.Value = 0;
            if (richTextBox1.Text == "")
            {
                var form = new Form();
                var label = new System.Windows.Forms.Label();
                form.Size = new System.Drawing.Size(200, 150);
                label.Text = "Please select original file first.";
                label.Size = new System.Drawing.Size(180, 20);
                form.Controls.Add(label);
                form.ShowDialog(this);
            }
            else
            {
                openFileDialog1.ShowDialog();
                string filename = openFileDialog1.FileName;
                richTextBox2.Text="";   // clear richTextBox2 on b2 click
                string originalText = richTextBox1.Text;
                string[] linesOfOrigin = originalText.Split(
                new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.RemoveEmptyEntries
                );
                string textToCheck = File.ReadAllText(filename);
                string[] linesToCheck = textToCheck.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.RemoveEmptyEntries
                    );
                textBox2.Text = filename;
                List<string[]> stringsToCompare = new List<string[]>();
                stringsToCompare.Add(linesOfOrigin);
                stringsToCompare.Add(linesToCheck);
                backgroundWorker1.RunWorkerAsync(stringsToCompare);
                backgroundWorker1.CancelAsync();
            }
            
        }

        private void backgroundWorker1_DoWork(object sender,
            DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = Compare((List<string[]>)e.Argument, worker, e);
        }

       /* private void backgroundWorker2_DoWork(object sender,
           DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = WriteToTextBox(
                (string)e.Argument, worker, e);
        } */

        private void backgroundWorker1_RunWorkerCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Invoke(() =>
            { progressBar1.Value = 0; });
            backgroundWorker1.CancelAsync();
        }

        void AppendText(RichTextBox box, Color color, string text)
        {
            int start = box.TextLength;
            box.AppendText(text);
            int end = box.TextLength;
            box.Select(start, end - start);
            {
                box.SelectionColor = color;
            }
            box.SelectionLength = 0;
        }
        private void Form1_Load(object sender, EventArgs e)
        { 

        }
    }
}