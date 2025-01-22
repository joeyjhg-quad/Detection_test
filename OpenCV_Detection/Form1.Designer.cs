namespace OpenCV_Detection
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lb_areaMin = new System.Windows.Forms.Label();
            this.lb_areaMax = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lb_roundMax = new System.Windows.Forms.Label();
            this.lb_roundMin = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lb_count = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.btn_baseImg = new System.Windows.Forms.Button();
            this.btn_inputImg = new System.Windows.Forms.Button();
            this.btn_inputImg_R = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(52, 41);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(802, 549);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(916, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "min :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(916, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "max :";
            // 
            // lb_areaMin
            // 
            this.lb_areaMin.AutoSize = true;
            this.lb_areaMin.Location = new System.Drawing.Point(956, 79);
            this.lb_areaMin.Name = "lb_areaMin";
            this.lb_areaMin.Size = new System.Drawing.Size(38, 12);
            this.lb_areaMin.TabIndex = 1;
            this.lb_areaMin.Text = "label1";
            // 
            // lb_areaMax
            // 
            this.lb_areaMax.AutoSize = true;
            this.lb_areaMax.Location = new System.Drawing.Point(956, 110);
            this.lb_areaMax.Name = "lb_areaMax";
            this.lb_areaMax.Size = new System.Drawing.Size(38, 12);
            this.lb_areaMax.TabIndex = 1;
            this.lb_areaMax.Text = "label1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(896, 334);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 18);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(896, 371);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(66, 18);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(875, 538);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(88, 21);
            this.textBox1.TabIndex = 4;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(875, 572);
            this.textBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(88, 21);
            this.textBox2.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(934, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "넓이";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(934, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "둘레";
            // 
            // lb_roundMax
            // 
            this.lb_roundMax.AutoSize = true;
            this.lb_roundMax.Location = new System.Drawing.Point(956, 207);
            this.lb_roundMax.Name = "lb_roundMax";
            this.lb_roundMax.Size = new System.Drawing.Size(38, 12);
            this.lb_roundMax.TabIndex = 7;
            this.lb_roundMax.Text = "label1";
            // 
            // lb_roundMin
            // 
            this.lb_roundMin.AutoSize = true;
            this.lb_roundMin.Location = new System.Drawing.Point(956, 177);
            this.lb_roundMin.Name = "lb_roundMin";
            this.lb_roundMin.Size = new System.Drawing.Size(38, 12);
            this.lb_roundMin.TabIndex = 8;
            this.lb_roundMin.Text = "label1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(916, 207);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 12);
            this.label7.TabIndex = 9;
            this.label7.Text = "max :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(916, 177);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 12);
            this.label8.TabIndex = 10;
            this.label8.Text = "min :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(914, 258);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "갯수 :";
            // 
            // lb_count
            // 
            this.lb_count.AutoSize = true;
            this.lb_count.Location = new System.Drawing.Point(956, 258);
            this.lb_count.Name = "lb_count";
            this.lb_count.Size = new System.Drawing.Size(38, 12);
            this.lb_count.TabIndex = 13;
            this.lb_count.Text = "label1";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(896, 404);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(117, 18);
            this.button3.TabIndex = 14;
            this.button3.Text = "침식팽창테스트";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(896, 441);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(82, 18);
            this.button4.TabIndex = 15;
            this.button4.Text = "템플릿매칭";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1013, 441);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 12);
            this.label6.TabIndex = 16;
            this.label6.Text = "label6";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(896, 478);
            this.button5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(117, 18);
            this.button5.TabIndex = 17;
            this.button5.Text = "템플릿매칭 각도";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // btn_baseImg
            // 
            this.btn_baseImg.Location = new System.Drawing.Point(896, 506);
            this.btn_baseImg.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_baseImg.Name = "btn_baseImg";
            this.btn_baseImg.Size = new System.Drawing.Size(66, 18);
            this.btn_baseImg.TabIndex = 18;
            this.btn_baseImg.Text = "base";
            this.btn_baseImg.UseVisualStyleBackColor = true;
            this.btn_baseImg.Click += new System.EventHandler(this.btn_baseImg_Click);
            // 
            // btn_inputImg
            // 
            this.btn_inputImg.Location = new System.Drawing.Point(970, 506);
            this.btn_inputImg.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_inputImg.Name = "btn_inputImg";
            this.btn_inputImg.Size = new System.Drawing.Size(66, 18);
            this.btn_inputImg.TabIndex = 19;
            this.btn_inputImg.Text = "input";
            this.btn_inputImg.UseVisualStyleBackColor = true;
            this.btn_inputImg.Click += new System.EventHandler(this.btn_inputImg_Click);
            // 
            // btn_inputImg_R
            // 
            this.btn_inputImg_R.Location = new System.Drawing.Point(970, 528);
            this.btn_inputImg_R.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_inputImg_R.Name = "btn_inputImg_R";
            this.btn_inputImg_R.Size = new System.Drawing.Size(66, 18);
            this.btn_inputImg_R.TabIndex = 20;
            this.btn_inputImg_R.Text = "input_R";
            this.btn_inputImg_R.UseVisualStyleBackColor = true;
            this.btn_inputImg_R.Click += new System.EventHandler(this.btn_inputImg_R_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1088, 636);
            this.Controls.Add(this.btn_inputImg_R);
            this.Controls.Add(this.btn_inputImg);
            this.Controls.Add(this.btn_baseImg);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.lb_count);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lb_roundMax);
            this.Controls.Add(this.lb_roundMin);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lb_areaMax);
            this.Controls.Add(this.lb_areaMin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lb_areaMin;
        private System.Windows.Forms.Label lb_areaMax;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lb_roundMax;
        private System.Windows.Forms.Label lb_roundMin;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lb_count;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button btn_baseImg;
        private System.Windows.Forms.Button btn_inputImg;
        private System.Windows.Forms.Button btn_inputImg_R;
    }
}

