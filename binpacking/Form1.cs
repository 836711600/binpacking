using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace binpacking
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            allfixed();
        }
        private void allfixed()
        {
            textBox3.Text = "";
            int area=0;
            List<Block> Blocks = new List<Block>();
            string[] tmparray=textBox2.Text.Split('x');
            Block packer = new Block() { x = 0, y = 0, w = int.Parse(tmparray[0]), h = int.Parse(tmparray[1]), used = false };
            Bitmap bmp = new Bitmap(int.Parse(tmparray[0]), int.Parse(tmparray[1]));
            Graphics g = Graphics.FromImage(bmp);
            g.DrawRectangle(new Pen(Color.Black), 0, 0, int.Parse(tmparray[0])-1, int.Parse(tmparray[1])-1);
            Block node;
            string[] lines = textBox1.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] array = line.Split('x');
                for (int i = 0; i < int.Parse(array[2]); i++)
                {
                    Blocks.Add(new Block() { x = 0, y = 0, w = int.Parse(array[0]), h = int.Parse(array[1]), sort = comboBox1.Text });
                }
            }
            Blocks.Sort();
            foreach (Block block in Blocks)
            {
                node = null;
                if ((node = findNode(packer, block.w, block.h)) != null)
                {
                    block.fit = splitNode(node, block.w, block.h);
                }
                if (block.fit != null)
                {
                    area = area + block.w * block.h;
                    g.DrawRectangle(new Pen(Color.Black), block.fit.x, block.fit.y, block.w, block.h);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(40, 255, 0, 0)), block.fit.x, block.fit.y, block.w, block.h);
                }
                else
                {
                    textBox3.AppendText(block.w.ToString() + "x" + block.h.ToString() + "\r\n");
                }
            }
            bmp.Save("allfixed.jpg");
            pictureBox1.Image = ReadImageFile("allfixed.jpg"); 
            textBox5.Text = ((double)area / (int.Parse(tmparray[0]) * int.Parse(tmparray[1]))).ToString("0.00%");
        }

        private void allfixedmulti()
        {
            int area = 0;
            List<Block> Blocks = new List<Block>();
            List<Block> tmpBlocks1,tmpBlocks2;
            string[] tmparray = textBox2.Text.Split('x');
            Block packer = new Block() { x = 0, y = 0, w = int.Parse(tmparray[0]), h = int.Parse(tmparray[1]), used = false };
            Bitmap bmp = new Bitmap(int.Parse(tmparray[0]), int.Parse(tmparray[1]));
            Graphics g = Graphics.FromImage(bmp);
            Block node;
            string[] lines = textBox1.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] array = line.Split('x');
                for (int i = 0; i < int.Parse(array[2]); i++)
                {
                    Blocks.Add(new Block() { x = 0, y = 0, w = int.Parse(array[0]), h = int.Parse(array[1]), sort = comboBox1.Text });
                }
            }
            int count=0;
            tmpBlocks2 = Blocks;
            do
            {
                count++;
                packer = new Block() { x = 0, y = 0, w = int.Parse(tmparray[0]), h = int.Parse(tmparray[1]), used = false };
                bmp = new Bitmap(int.Parse(tmparray[0]), int.Parse(tmparray[1]));
                g = Graphics.FromImage(bmp);
                g.DrawRectangle(new Pen(Color.Black), 0, 0, int.Parse(tmparray[0])-1, int.Parse(tmparray[1])-1);
                tmpBlocks1=tmpBlocks2;
                tmpBlocks2=new List<Block>();
                tmpBlocks1.Sort();
                foreach (Block block in tmpBlocks1)
                {
                    node = null;
                    if ((node = findNode(packer, block.w, block.h)) != null)
                    {
                        block.fit = splitNode(node, block.w, block.h);
                    }
                    if (block.fit != null)
                    {
                        area = area + block.w * block.h;
                        g.DrawRectangle(new Pen(Color.Black), block.fit.x, block.fit.y, block.w, block.h);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(40, 255, 0, 0)), block.fit.x, block.fit.y, block.w, block.h);
                    }
                    else
                    {
                        tmpBlocks2.Add(block);
                    }
                }
                bmp.Save("allfixedmulti"+count.ToString()+".jpg");
            }while(tmpBlocks2.Count()>0);
            pictureBox1.Image = ReadImageFile("allfixedmulti1.jpg");
            textBox6.Text = count.ToString();
            textBox7.Text = ((double)area / (int.Parse(tmparray[0]) * int.Parse(tmparray[1])*count)).ToString("0.00%");
        }

        private void autochange()
        {
            int area = 0;
            List<Block> Blocks = new List<Block>();
            Block node;
            string[] lines = textBox1.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] array = line.Split('x');
                for (int i = 0; i < int.Parse(array[2]); i++)
                {
                    Blocks.Add(new Block() { x = 0, y = 0, w = int.Parse(array[0]), h = int.Parse(array[1]), sort = comboBox1.Text });
                }
            }
            Blocks.Sort();
            Block root = new Block() { x = 0, y = 0, w = Blocks[0].w, h = Blocks[0].h, used = false };
            foreach (Block block in Blocks)
            {
                node = null;
                if ((node = findNode(root, block.w, block.h)) != null)
                {
                    block.fit = splitNode(node, block.w, block.h);
                }
                else
                {
                    block.fit = growNode(root,block.w, block.h);
                }
            }
            Bitmap bmp = new Bitmap(root.w, root.h);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawRectangle(new Pen(Color.Black), 0, 0, root.w-1, root.h-1);
            foreach (Block block in Blocks)
            {
                if (block.fit != null)
                {
                    area = area + block.w * block.h;
                    g.DrawRectangle(new Pen(Color.Black), block.fit.x, block.fit.y, block.w, block.h);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(40, 255, 0, 0)), block.fit.x, block.fit.y, block.w, block.h);
                }
            }
            bmp.Save("autochange.jpg");
            pictureBox1.Image = ReadImageFile("autochange.jpg");
            textBox8.Text = root.w.ToString()+ "x"+ root.h.ToString();
            textBox9.Text = ((double)area / (root.w * root.h)).ToString("0.00%");
        }

        private void heightfixed()
        {
            int area=0;
            List<Block> Blocks = new List<Block>();
            Block node;
            string[] lines = textBox1.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] array = line.Split('x');
                for (int i = 0; i < int.Parse(array[2]); i++)
                {
                    Blocks.Add(new Block() { x = 0, y = 0, w = int.Parse(array[0]), h = int.Parse(array[1]), sort = comboBox1.Text });
                }
            }
            Blocks.Sort();
            Block root = new Block() { x = 0, y = 0, w = Blocks[0].w, h = Blocks[0].h, used = false };
            foreach (Block block in Blocks)
            {
                node = null;
                if ((node = findNode(root, block.w, block.h)) != null)
                {
                    block.fit = splitNode(node, block.w, block.h);
                }
                else
                {
                    block.fit = heightfixedgrowNode(root, block.w, block.h,int.Parse(textBox4.Text));
                }
            }
            Bitmap bmp = new Bitmap(root.w, int.Parse(textBox4.Text));
            Graphics g = Graphics.FromImage(bmp);
            g.DrawRectangle(new Pen(Color.Black), 0, 0, root.w - 1, int.Parse(textBox4.Text) - 1);
            foreach (Block block in Blocks)
            {
                if (block.fit != null)
                {
                    area = area + block.w * block.h;
                    g.DrawRectangle(new Pen(Color.Black), block.fit.x, block.fit.y, block.w, block.h);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(40, 255, 0, 0)), block.fit.x, block.fit.y, block.w, block.h);
                }
            }
            bmp.Save("heightfixed.jpg");
            pictureBox1.Image = ReadImageFile("heightfixed.jpg");
            textBox10.Text = root.w.ToString();
            textBox11.Text = ((double)area / (root.w * int.Parse(textBox4.Text))).ToString("0.00%");
        }

        private Block heightfixedgrowNode(Block root, int w, int h,int limit)
        {
            Boolean canGrowDown, canGrowRight, shouldGrowDown, shouldGrowRight;
            canGrowDown = (h+root.h<=limit);
            canGrowRight = true;
            shouldGrowRight = canGrowRight && (root.h >= (root.w + w));
            shouldGrowDown = canGrowDown && (root.w >= (root.h + h));
            if (shouldGrowDown)
                return growDown(root, w, h);
            else if (shouldGrowRight)
                return growRight(root, w, h);
            else if (canGrowDown)
                return growDown(root, w, h);
            else if (canGrowRight)
                return growRight(root, w, h);
            else
                return null;
        }

        private Block growNode(Block root, int w, int h)
        {
            Boolean canGrowDown, canGrowRight, shouldGrowDown, shouldGrowRight;
            canGrowDown = (w <= root.w);
            canGrowRight = (h <= root.h);
            shouldGrowRight = canGrowRight && (root.h >= (root.w + w));
            shouldGrowDown = canGrowDown && (root.w >= (root.h + h));
            if (shouldGrowRight)
                return growRight(root,w, h);
            else if (shouldGrowDown)
                return growDown(root,w, h);
            else if (canGrowRight)
                return growRight(root,w, h);
            else if (canGrowDown)
                return growDown(root,w, h);
            else
                return null;
        }

        private Block growRight(Block root, int w, int h)
        {
            Block node;
            Block rootcopy=new Block();
            rootcopy.x=root.x;
            rootcopy.y=root.y;
            rootcopy.w=root.w;
            rootcopy.h=root.h;
            rootcopy.used=root.used;
            rootcopy.down=root.down;
            rootcopy.right=root.right;
            root.used=true;
            root.x=0;
            root.y=0;
            root.w=rootcopy.w+w;
            root.h=rootcopy.h;
            root.down=rootcopy;
            root.right=new Block() { x = rootcopy.w, y = 0, w = w, h = rootcopy.h,used=false };
            if ((node = findNode(root, w, h))!=null)
                return this.splitNode(node, w, h);
            else
                return null;
        }

        private Block growDown(Block root, int w, int h)
        {
            Block node;
            Block rootcopy = new Block();
            rootcopy.x = root.x;
            rootcopy.y = root.y;
            rootcopy.w = root.w;
            rootcopy.h = root.h;
            rootcopy.used = root.used;
            rootcopy.down = root.down;
            rootcopy.right = root.right;
            root.used = true;
            root.x = 0;
            root.y = 0;
            root.w = rootcopy.w;
            root.h = rootcopy.h+h;
            root.down = new Block() { x = 0, y = rootcopy.h, w = rootcopy.w, h = h, used = false };
            root.right = rootcopy;
            if ((node = findNode(root, w, h)) != null)
                return this.splitNode(node, w, h);
            else
                return null;
        }

        private Block findNode(Block root,int w,int h)
        {
            Block right,down;
            if (root.used)
            {
                if ((right = findNode(root.right, w, h)) != null)
                    return right;
                else if((down=findNode(root.down, w, h))!=null)
                    return down;
                else return null;
            }
            else if ((w <= root.w) && (h <= root.h))
                return root;
            else
                return null;
        }
        private Block splitNode(Block node, int w, int h)
        {
            node.used = true;
            node.down  = new Block() { x =  node.x, y = node.y + h, w = node.w, h = node.h - h,used=false };
            node.right  = new Block() { x =  node.x + w, y = node.y, w = node.w - w, h = h,used=false };
            return node;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            autochange();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            allfixedmulti();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            heightfixed();
        }

        public static Bitmap ReadImageFile(string path)
        {
            FileStream fs = File.OpenRead(path); //OpenRead
            int filelength = 0;
            filelength = (int)fs.Length; //获得文件长度 
            Byte[] image = new Byte[filelength]; //建立一个字节数组 
            fs.Read(image, 0, filelength); //按字节流读取 
            System.Drawing.Image result = System.Drawing.Image.FromStream(fs);
            fs.Close();
            Bitmap bit = new Bitmap(result);
            return bit;
        }
    }
    public class Block : IComparable<Block>
    {
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public string sort { get; set; }
        public Block fit  { get; set; }
        public Boolean used { get; set; }
        public Block down { get; set; }
        public Block right { get; set; }
        public int CompareTo(Block obj)
        {
            switch (sort)
            {
                case "width": return this.w.CompareTo(obj.w)*(-1);
                case "height": return this.h.CompareTo(obj.h) * (-1);
                case "maxside": return Math.Max(this.w, this.h).CompareTo(Math.Max(obj.w, obj.h)) * (-1);
                case "area": return (this.w*this.h).CompareTo(obj.w*obj.h) * (-1);
                default: return this.w.CompareTo(obj.w) * (-1);
            }
        }
    }
}
