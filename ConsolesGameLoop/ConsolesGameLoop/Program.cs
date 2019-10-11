using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolesGameLoop
{
    class Program
    {

        public static Game m_game;
        public static ConsoleKeyInfo Input_Key = new ConsoleKeyInfo();
        public static bool Is_Input_Key_Down = false;
        static void Main(string[] args)
        {
            m_game = new Game();
            m_game.Game_Init();
            string oldstr_panel = "";
            var lastTime = DateTime.Now;
            double deltaTime = 0;
            while (true)
            {
                deltaTime = (DateTime.Now - lastTime).TotalMilliseconds / 1000;
                lastTime = DateTime.Now;

                //非阻塞读取键盘输入
                if (Console.KeyAvailable)
                {
                    Input_Key = Console.ReadKey(true);
                    Is_Input_Key_Down = true;
                }
                else
                {
                    Is_Input_Key_Down = false;
                }
                //游戏循环
                m_game.Game_Update(deltaTime);

                //画面渲染
                string str_panel = m_game.Game_Draw();
                if (str_panel != oldstr_panel)
                {
                    oldstr_panel = str_panel;
                    Console.Clear();
                    Console.Write(oldstr_panel);
                }
            }
        }
    }

    public class Point
    {
        public int x;
        public int y;
    }
    //游戏逻辑类
    public class Game
    {
        public Point[] player_points; //蛇的全身身体
        public int player_score = 0;  //蛇的分数 
        public int player_flag = 2;  //1 上 2 下 3 左 4 右

        public int map_width = 0;
        public int map_height = 0;

        public Point apple_point;

        private double player_move_timer = 0;
        private string[][] map_chars;
        private Random rnd = new Random();


        public bool isGameStart = false;

        //游戏初始化
        public void Game_Init()
        {
            map_height = 10;
            map_width = 10;
            map_chars = new string[map_height][];
            for (int i = 0; i < map_height; i++)
            {
                map_chars[i] = new string[map_width];
            }

            player_points = new Point[map_height * map_width];
            player_points[0] = new Point() { x = 4, y = 4 };
            apple_point = new Point() { x = 0, y = 0 };
        }

        //游戏渲染
        public string Game_Draw()
        {

            if (isGameStart)
            {
                Map_Draw();
                Player_Draw();
                Apple_Draw();
            }
            else
            {
                TilePanel_Draw();
            }
            string str_map = "";
            int i = 0;
            for (int y = 0; y < map_height; y++)
            {
                for (int x = 0; x < map_width; x++)
                {
                    str_map += map_chars[y][x];
                }
                str_map += "\n";
            }
            return str_map;
        }

        private void TilePanel_Draw()
        {
            Set_Map(1,0,"按");
            Set_Map(2,0,"J");
            Set_Map(3,0,"键");
            Set_Map(4,0,"开");
            Set_Map(5,0,"始");
        }

        private void Apple_Draw()
        {
            Set_Map(apple_point.x, apple_point.y, "○");
        }

        public void Game_Update(double deltaTime)
        {
            if (isGameStart)
            {
                player_move_timer += deltaTime;
                if (player_move_timer >= 0.2)
                {
                    player_move_timer = 0;
                    Player_Movement();
                }
            }
            if (Program.Is_Input_Key_Down)
            {
                switch (Program.Input_Key.KeyChar)
                {
                    case 'w': if (isGameStart) player_flag = 1; break;
                    case 's': if (isGameStart) player_flag = 2; break;
                    case 'a': if (isGameStart) player_flag = 3; break;
                    case 'd': if (isGameStart) player_flag = 4; break;
                    case 'j': isGameStart = true; break;
                }
            }
        }

        public void Player_Movement()
        {
            for (int i = player_points.Length - 1; i > 0; i--)
            {
                if (player_points[i] != null)
                {
                    player_points[i].x = player_points[i - 1].x;
                    player_points[i].y = player_points[i - 1].y;
                }
            }
            switch (player_flag)
            {
                case 1: player_points[0].y--; break;
                case 2: player_points[0].y++; break;
                case 3: player_points[0].x--; break;
                case 4: player_points[0].x++; break;
            }
            if (player_points[0].x == apple_point.x && player_points[0].y == apple_point.y)
            {
                Player_Eat_Apple();
            }
        }

        public void Player_Eat_Apple()
        {
            player_score++;
            for (int i = 0; i < player_points.Length; i++)
            {
                if (player_points[i] == null)
                {
                    player_points[i] = new Point()
                    {
                        x = player_points[i - 1].x,
                        y = player_points[i - 1].y
                    };
                    break;
                }
            }
            Random_Apple_Point();
        }

        public void Random_Apple_Point()
        {
            List<Point> points = new List<Point>();
            for (int y = 0; y < map_height; y++)
            {
                for (int x = 0; x < map_width; x++)
                {
                    if (map_chars[y][x] != "■")
                    {
                        points.Add(new Point { x = x, y = y });
                    }
                }
            }
            apple_point = points[rnd.Next(points.Count)];
        }

        private void Map_Draw()
        {
            for (int y = 0; y < map_height; y++)
            {
                for (int x = 0; x < map_width; x++)
                {
                    Set_Map(x, y, "..");
                }
            }

        }

        private void Player_Draw()
        {
            for (int i = 0; i < player_points.Length; i++)
            {
                if (player_points[i] != null)
                {
                    Set_Map(player_points[i].x, player_points[i].y, "■");
                }
            }
        }

        public void Set_Map(int x, int y, string info)
        {
            if (x < 0 || x >= map_width || y < 0 || y >= map_height) return;
            map_chars[y][x] = info;
        }
    }
}