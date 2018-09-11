using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace TCApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AppendText(this.richTextBox3, Color.Green, "Unchanged text is green. ");
            AppendText(this.richTextBox3, Color.Orange, "Added/Edited text is orange. ");
            AppendText(this.richTextBox3, Color.Red, "Deleted text is red.");
        }

        private void compareAsync(string[] linesOfOrigin, string[] linesToCheck, int[] removed_indexes, List<Colorator> finalList)
        {
            for (int i = 0; i < linesOfOrigin.Length; i++)              // initialize array of removed original indexes
                removed_indexes[i] = 1;
            for (int i = 0; i < linesToCheck.Length; i++)
            {
                bool found = false;
                for (int j = 0; j < linesOfOrigin.Length; j++)
                {
                    if ((linesToCheck[i] == linesOfOrigin[j]) & (!found) && (removed_indexes[j] != 0))
                    {
                        Colorator colorator = new Colorator(linesOfOrigin[j], Color.Green);
                        finalList.Add(colorator);                     // insert colored text into list
                        removed_indexes[j] = 0;
                        found = true;
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
                            break;
                        }
                    }
                }

                if (!found)
                {
                    Colorator colorator = new Colorator(linesToCheck[i], Color.Orange);
                    finalList.Add(colorator);
                }
            }
            
            for (int i = 0; i < linesOfOrigin.Length; i++)
            {
                if (removed_indexes[i] == 1)
                {
                    Colorator colorator = new Colorator(linesOfOrigin[i], Color.Red);
                    finalList.Insert(i + 1, colorator);            // insert colored text into list
                }
            }
        }

        private async System.Threading.Tasks.Task ComputeAsync(string[] origin, string[] changed, int[] rem, List<Colorator> finalList)
        {
            compareAsync(origin, changed, rem, finalList);
        }

        private async void compare(string filename)
        {
            richTextBox2.Text = "";                                      // clear richTextBox2 on b2 click
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
            List<Colorator> finalList = new List<Colorator>();
            int[] removed_indexes = new int[linesOfOrigin.Length];

            await System.Threading.Tasks.Task.Run(() => ComputeAsync(linesOfOrigin, linesToCheck, removed_indexes,finalList));
            for (int i = 0; i<finalList.Count(); i++)
                AppendText(richTextBox2, finalList[i].color, finalList[i].text +"\n");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string filename = openFileDialog1.FileName;
            string readFile = File.ReadAllText(filename);
            richTextBox1.Text = readFile;
            textBox1.Text = filename;
            if (richTextBox2.Text != "")
            {
                compare(filename);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
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
                compare(filename);
            }
            
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

    }
}

class Worker
{
    public static void SomeLongOperation(IProgress<string> progress)
    {
        // Perform a long running work...
        for (var i = 0; i < 10; i++)
        {
            Task.Delay(500).Wait();
            progress.Report(i.ToString());
        }
    }
}