# ConsolesGameLoop
基于C#控制台的游戏循环机制


通过在Main方法下的while循环实现游戏循环,并通过  Console.KeyAvailable属性 来实现非阻塞地实时接收键盘输入

while(true){
    if(Console.KeyAvailable){
        var input_char = Console.ReadKey(true);
    }
}
