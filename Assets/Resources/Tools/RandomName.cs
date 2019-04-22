
using System;

public class RandomName
{
    
    //Genere un nom de joueur aleatoire
    public static string Generate()
    {
        Random rnd = new Random();
        Array values = Enum.GetValues(typeof(Words));
        string randomword =((Words)values.GetValue(rnd.Next(values.Length))).ToString();

        Array randomadjectives = Enum.GetValues(typeof(Words));
        string randomadjective = ((Words)randomadjectives.GetValue(rnd.Next(randomadjectives.Length))).ToString();

        return randomword + " " + randomadjective;
    }
    
    //Genere un nom d'IA aleatoire
    public static string GenerateAI()
    {
        Random rnd = new Random();
        Array randomadjectives = Enum.GetValues(typeof(Words));
        string randomadjective = ((Words)randomadjectives.GetValue(rnd.Next(randomadjectives.Length))).ToString();

        return randomadjective + " IA";

    }

    public enum Words
    {
        Spider,
        Eagle,
        Man,
        Woman,
        Juice,
        Perfume,
        Spary,
        Paper,
        Cream,
        Bowl,
        Mug,
        Screen,
        Librarian,
        Tissue,
        Brush,
        Archer,
        Shoe,
        Leaf,
        River,
        Snow,
        Mouth,
        Field,
        Laugh,
        Pumpkin,
        Sunflower,
        Grandmother,
        Grandfather,
        Oldman,
        Oldwoman,
        Cube,
        Fruit,
        Pie,
        Goat,
        Flower,
        Spoon,
        Cheese,
        Onion,
        Astronaut,
        Doctor, 
        Pond,
        Melody,
        Runner,
        Sea,
        Cow,
        Seat,
        Herb,
        Heart,
        Fish,
        Goldfish,
        Nemo,
        Bear,
        Dog,
        Cat,
        Slippers,
        Ball,
        Summer,
        Winter,
        Spring,
        Autumn,
        Wall,
        Arrow,
        Target,
        Bull,
        Eye,
        Hand,
        Foot,
        Glass,
        Rose,
        Road,
        Sword,
        Hammer,
        Claw,
        Conqueror,
        Pizza,
        Pasta,
        Ham,
        Belt,
        Helmet,
        Lamp,
        Pen,
        Dragon,
        Robot,
        Cyborg,
        Thunder,
        Panda,
        Ghost,
        Gambler,
        Smile

    };

    public enum Adjectives
    {
        Random,
        Idiot,
        Stupid,
        Clever,
        Beautiful,
        Ugly,
        Big,
        Small,
        Weak,
        Strong,
        Red,
        Blue,
        Orange,
        Yellow,
        Black,
        White,
        Dark,
        Light,
        Fire,
        Water,
        Grass,
        Fat,
        Wooden,
        Lightning,
        Slow,
        Fast,
        Speedy,
        Best,
        Scared,
        Brave,
        Sweet,
        Salty,
        Cheesy,
        Laughing,
        Witty,
        Hungry,
        Happy,
        Angry,
        Sleepy,
        Dying,
        Undead,
        Glass,
        Grassy,
        Ice,
        Wild,
        Savage,
        Funny,
        Dirty,
        Rolling,
        Flying,
        Unlucky,
        Lucky,
        One_eyed,
        Bloody,
        Rich,
        Poor,
        Lonely,
        Sad,
        Invisible

    };
}
