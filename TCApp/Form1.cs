using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;

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

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string filename = openFileDialog1.FileName;
            string readFile = File.ReadAllText(filename);
            richTextBox1.Text = readFile;
            textBox1.Text = filename;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "")
            {
                var form = new Form();
                var label = new System.Windows.Forms.Label();
                form.Size = new System.Drawing.Size(200,150);
                label.Text = "Please select original file first.";
                label.Size = new System.Drawing.Size(180, 20);
                form.Controls.Add(label);
                form.ShowDialog(this);
            }
            else
            {
                richTextBox2.Text = "";                                      // clear richTextBox2 on b2 click
                string originalText = richTextBox1.Text;
                string[] linesOfOrigin = originalText.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.RemoveEmptyEntries
                    );
                string[] lostLines = new String[linesOfOrigin.Length];
                for (int i = 0; i < linesOfOrigin.Length; i++)
                    lostLines[i] = linesOfOrigin[i];
                openFileDialog1.ShowDialog();
                string filename = openFileDialog1.FileName;
                string textToCheck = File.ReadAllText(filename);
                string[] linesToCheck = textToCheck.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.RemoveEmptyEntries
                    );
                textBox2.Text = filename;
                List<Colorator> finalList = new List<Colorator>();
                int indexSize = linesOfOrigin.Length;
                int[] removed_indexes = new int[indexSize];
                
                for (int i = 0; i < indexSize; i++)              // initialize array of removed original indexes
                    removed_indexes[i] = 1;

                for (int i = 0; i < linesToCheck.Length; i++)
                {
                    bool found = false;
                    for (int j = 0; j < linesOfOrigin.Length; j++)
                    {
                        if ((linesToCheck[i] == linesOfOrigin[j])&(!found)&&(removed_indexes[j] != 0))
                        {
                            Colorator colorator = new Colorator(linesToCheck[i], Color.Green);
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
                            if ((words.Any(linesOfOrigin[j].Contains))&&(removed_indexes[j] != 0))
                            {
                                Colorator colorator = new Colorator(linesToCheck[i], Color.Orange);
                                finalList.Add(colorator);          

                                Colorator colorator2 = new Colorator(linesOfOrigin[j], Color.Red);
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

                for (int i = 0; i < lostLines.Length; i++)
                    {
                        if (removed_indexes[i] == 1)
                        {
                            Colorator colorator = new Colorator(lostLines[i], Color.Red);
                            finalList.Insert(i,colorator);            // insert colored text into list
                        }
                    }

                for (int i = 0; i < finalList.Count(); i++)
                    AppendText(richTextBox2, finalList[i].color,finalList[i].text + "\n");
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

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
