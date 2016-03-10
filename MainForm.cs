using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundCloudBot
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.textBox1.KeyDown += (o, e) => { if (e.KeyCode == Keys.Enter) button1_Click(null, null); };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0) 
                listBox1.Items.Add(textBox1.Text);
            textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Remove(listBox1.SelectedItem);
        }

        public void log(string text)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate() {
                    if (listBox2.Items.Count >= 100)
                        listBox2.Items.RemoveAt(0);
                    listBox2.Items.Add(text);
                    int visibleItems = listBox2.ClientSize.Height / listBox2.ItemHeight;
                    listBox2.TopIndex = Math.Max(listBox2.Items.Count - visibleItems + 1, 0);
                });
            }
            catch
            {
                listBox2.Items.Add(text);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                log("Testing OAuth...");
                foreach (object item in listBox1.Items)
                    log(item + " is " + (new SoundCloud((string)item).Search("X") == null ? "in" : "") + "valid");
            }).Start();
        }

        public string GetKey(int index)
        {
            string[] keys = richTextBox1.Text.Split(',');
            if (checkBox1.Checked)
                index = new Random().Next(keys.Length - 1);
            Program.GUI.log("New Key " + keys[index]);
            return keys[index];
        }

        public string GetComment(int index)
        {
            string[] keys = richTextBox3.Text.Split(new[]{"====="}, StringSplitOptions.RemoveEmptyEntries);
            if (checkBox2.Checked)
                index = new Random().Next(keys.Length - 1);
            return keys[index];
        }

        public bool IsRandomID()
        {
            return checkBox3.Checked;
        }

        public bool IsRandomComment()
        {
            return checkBox2.Checked;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new Thread(() => {
                try
                {
                    foreach(var item in listBox1.Items) 
                        new LikeSession(new SoundCloud((string)item)).Start(int.Parse(textBox2.Text), int.Parse(textBox7.Text));
                }
                catch {
                    MessageBox.Show("Configure the likes settings!");
                }
            }).Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    foreach (var item in listBox1.Items)
                        new FollowSession(new SoundCloud((string)item)).Start(int.Parse(textBox3.Text), int.Parse(textBox6.Text));
                }
                catch {
                    MessageBox.Show("Configure the follow settings!");
                }
            }).Start();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    foreach (var item in listBox1.Items)
                        new CommentSession(new SoundCloud((string)item)).Start(int.Parse(textBox4.Text), int.Parse(textBox5.Text));
                }
                catch
                {
                    MessageBox.Show("Configure the comments settings!");
                }
            }).Start();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    log("Unfollowing all...");
                    foreach (var item in listBox1.Items)
                    {
                        var sc = new SoundCloud((string)item);
                        if (File.Exists(".\\followed\\" + sc.GetOAuth() + ".txt"))
                        {
                            foreach (var line in File.ReadAllLines(".\\followed\\" + sc.GetOAuth() + ".txt"))
                            {
                                if (line.Length < 3) continue;
                                sc.UnFollow(line);
                                log("Unfollowed " + line);
                            }
                            File.Delete(".\\followed\\" + sc.GetOAuth() + ".txt");
                        }
                    }
                }
                catch
                {

                }
                log("Finished unfollowing");
            }).Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    log("Unliking all...");
                    foreach (var item in listBox1.Items)
                    {
                        var sc = new SoundCloud((string)item);
                        if (File.Exists(".\\liked\\" + sc.GetOAuth() + ".txt"))
                        {
                            foreach (var line in File.ReadAllLines(".\\liked\\" + sc.GetOAuth() + ".txt"))
                            {
                                if (line.Length < 3) continue;
                                sc.UnLikeTrack(line);
                                log("Unliked " + line);
                            }
                            File.Delete(".\\liked\\" + sc.GetOAuth() + ".txt");
                        }
                    }
                }
                catch
                {

                }
                log("Finished unliking");
            }).Start();
        }
    }
}
