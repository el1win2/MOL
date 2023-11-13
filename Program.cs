using System.Numerics;
using System.Reflection.Metadata;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// 캐릭터 인터페이스 설정
public interface ICharacter
{
    string Name { get; }
    int Health { get; }
    int Attack { get; }
    int HP { get; set; }
    int AD { get; }
    int AC { get; }
    bool IsDead { get; }
    void TakeDamage(int damage);
}

// 전사 클래스 설정
public class Warrior : ICharacter
{
    // 값이 변하지 않는 기본 스테이터스
    public string Name { get; }
    public int Health { get; set; }
    public int Attack { get; set; }
    public int HP { get; set; }
    public int AC { get; set; }
    // 전투불능 체크
    public bool IsDead => HP <= 0;
    // 초기 설정
    public Warrior(string name)
    {
        Name = name;
        Health = 100;
        Attack = 10;
        HP = Health;
        AC = 0;
    }
    // 값이 변하는 추가 스테이터스
    public int AD => new Random().Next((int)(Attack * 0.5), Attack);
    // 피해 교환
    public void TakeDamage(int damage)
    {
        int dps = damage - AC;
        if(dps <= 1) { dps = 1; }
        HP -= dps;
        if (IsDead)
        {
            HP = 0;
            Console.WriteLine($"{Name}은 {dps}의 피해를 받고 체력이 {HP}이 되어 쓰러지게 됩니다.");
        }
        else
        {
            Console.WriteLine($"{Name}은 {dps}의 피해를 받고 체력이 {HP}이 됩니다.");
        }
    }
}

// 몬스터 클래스 설정
public class Monster : ICharacter
{
    // 값이 변하지 않는 기본 스테이터스
    public string Name { get; }
    public int Health { get; }
    public int Attack { get; }
    public int HP { get; set; }
    public int AC { get; }
    // 전투불능 체크
    public bool IsDead => HP <= 0;
    // 초기 설정
    public Monster(string name, int health, int attack, int ac)
    {
        Name = name;
        Health = health;
        Attack = attack;
        HP = Health;
        AC = ac;
    }
    // 값이 변하는 추가 스테이터스
    public int AD => new Random().Next((int)(Attack * 0.5), Attack);
    // 피해 교환
    public void TakeDamage(int damage)
    {
        // 전사로부터 받는 공격의 치명타 체크
        int Critical = new Random().Next(1, 11);
        if (Critical >= 9)
        {
            Console.WriteLine($"치명적인 일격 발생! {damage}가 2배가 됩니다!");
            damage *= 2;
        }
        int dps = damage - AC;
        if (dps <= 1) { dps = 1; }
        HP -= dps;
        if (IsDead)
        {
            HP = 0;
            Console.WriteLine($"{Name}은 {dps}의 피해를 받고 체력이 {HP}이 되어 쓰러지게 됩니다.");
        }
        else
        {
            Console.WriteLine($"{Name}은 {dps}의 피해를 받고 체력이 {HP}이 됩니다.");
        }
    }
}
// 고블린 클래스 생성
public class Goblin : Monster
{
    //고블린의 이름, 체력 50, 공격력 10, 방어력 0 { } 안은 사용하지 않음.
    public Goblin(string name) : base(name, 50, 10, 0) { }
}

// 고블린 클래스 생성
public class Dragon : Monster
{
    //드래곤의 이름, 체력 150, 공격력 20, 방어력 5 { } 안은 사용하지 않음.
    public Dragon(string name) : base(name, 150, 20, 5) { }
}

// 아이템 인터페이스 설정
public interface IItem
{
    public string Name { get; }
    public int HPValue { get; }
    public int ACValue { get; }
    public int AttackValue { get; }

    // 워리어가 아이템을 사용할 시
    public void Use(Warrior warrior);
}
// 장비
// 한손검 설정
public class Sword : IItem
{
    public string Name { get; }
    public int HPValue { get; }
    public int ACValue { get; }
    public int AttackValue { get; }

    // 워리어가 아이템을 사용할 시
    public void Use(Warrior warrior)
    {

    }
}
// 방패
// 갑옷

// 소모품
// 체력 회복 물약 설정
public class HealthPotion : IItem
{
    public string Name => "체력 증가 물약";
    public int HPValue => 0;
    public int ACValue => 0;
    public int PotionValue => 100;
    public int AttackValue => 0;
    // 헬스포션 사용 시 효과 설정
    public void Use(Warrior warrior)
    {
        Console.WriteLine($"{warrior.Name}의 체력이 {PotionValue} 증가합니다.");
        Console.WriteLine("저 전사는 피가 프로틴으로 이루어져 있겠군!");
        warrior.Health += PotionValue;
    }
}

// 체력 회복 물약 설정
public class HealingPotion : IItem
{
    public string Name { get; }
    public int HPValue => 0;
    public int ACValue => 0;
    public int PotionValue { get; }
    public int AttackValue => 0;
    // 헬스포션 사용 시 효과 설정
    public HealingPotion(int value)
    {
        Name = $"체력 {value} 물약";
        PotionValue = value;
    }
    public void Use(Warrior warrior)
    {
        Console.WriteLine($"{warrior}의 체력이 {PotionValue} 만큼 회복합니다.");
        Console.WriteLine("저 전사의 몸에 생기가 돌아왔다!");
        warrior.HP += PotionValue;
        if(warrior.HP >= warrior.Health)
        {
            warrior.HP = warrior.Health;
        }
    }
}
// 공격력 증가 물약 설정
public class StrengthPotion : IItem
{
    public string Name => "근육 뻠핑 물약";
    public int HPValue => 0;
    public int ACValue => 0;
    public int PotionValue => 0;
    public int AttackValue => 10;
    // 스트렝스포션 사용시 효과 설정
    public void Use(Warrior warrior)
    {
        Console.WriteLine($"{warrior.Name}의 공격력이 {AttackValue} 증가합니다.");
        Console.WriteLine("저 전사는 오늘 이두박근을 안 조져도 되겠군!");
        warrior.Attack += AttackValue;
    }
}

// 힐링포션 설정
public class Heal50P : HealingPotion
{
    //힐 수치 { } 안은 사용하지 않음.
    public Heal50P() : base(50) { }
}
// 스테이지 설정
public class Stage
{
    // 델리게이트 설정
    public delegate void DeathEvent(ICharacter character);
    public event DeathEvent Death;

    // 기본 설정
    public ICharacter warrior;
    public ICharacter monster;
    public List<IItem> dropTable;
    // 캐릭터 값 설정
    public Stage(ICharacter warrior, ICharacter monster, List<IItem> dropTable)
    {
        this.warrior = warrior;
        this.monster = monster;
        this.dropTable = dropTable;
        Death += End;
    }

    // 스테이지 시작
    public bool Start()
    {
        // 기본 세팅
        int turnCharacter = 0;
        int dps;
        bool IsBool = false;

        // 시작
        Console.WriteLine($"당신의 능력치는 체력 {warrior.Health}, 공격력 {warrior.Attack} 입니다.");
        Console.WriteLine($"상대는 {monster.Name} ! 체력은 {monster.Health} 이며, 공격력은 {monster.Attack} 입니다.");

        // 게임 시작
        while (!warrior.IsDead && !monster.IsDead)
        {
            // 턴 결정
            if (turnCharacter == 0)
            {
                Console.WriteLine($"{warrior.Name}의 턴입니다.");
                monster.TakeDamage(warrior.AD);
                Console.WriteLine();
                // 몬스터 전투불능 확인
                if (monster.IsDead)
                {
                    break;
                }
                // 턴 변경
                turnCharacter = 1;
            }
            else
            {
                Console.WriteLine($"{monster.Name}의 턴입니다.");
                warrior.TakeDamage(monster.AD);
                Console.WriteLine();
                // 플레이어 전투불능 확인
                if (warrior.IsDead)
                {
                    break;
                }
                // 턴 변경
                turnCharacter = 0;
            }
            // 시간 설정 1초
            Thread.Sleep(1000);
        }
        // 전투불능 확인 유무
        if (monster.IsDead)
        {
            Death?.Invoke(monster);
            IsBool = true;

        }
        else if (warrior.IsDead)
        {
            Death?.Invoke(warrior);
        }
        // 재 대기시간
        Thread.Sleep(1000);
        // 결과값 리턴
        return IsBool;
    }

    // 스테이지 종료
    public void End(ICharacter character)
    {
        // 죽은 게 적인지 자신인지 확인
        if (character is Monster)
        {
            // 적 격파 성공
            Console.WriteLine("적을 쓰러트려 이 지역을 정복했습니다! 보상을 선택해주세요!");
            foreach (var drop in dropTable)
            {
                // 보상 안내
                Console.WriteLine(drop.Name);
            }

            // 보상 입력 칸
            Console.Write("받을 보상의 이름을 적어주세요: ");
            string input = Console.ReadLine();
            // 아이템 효과 발동
            IItem inputItem = dropTable.Find(item => item.Name == input);
            if (inputItem != null)
            {
                inputItem.Use((Warrior)warrior);
                // 사용 아이템 정보
                if (inputItem is HealthPotion)
                {
                    Console.WriteLine($"{inputItem}을 사용했습니다. 최대 체력이 100 증가합니다.");
                }
                else if (inputItem is StrengthPotion)
                {
                    Console.WriteLine($"{inputItem}을 사용했습니다. 공격력이 10 증가합니다.");
                }
            }
        }
        else
        {
            // 적 격파 실패
            Console.WriteLine("적을 쓰러트리지 못했습니다. 다시 시작해주세요...");
        }
        // 최대 체력으로 회복
        warrior.HP = warrior.Health;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // 계정 등록 및 확인
        string Registration(string id, string name)
        {
            // 기본값 생성
            JObject account = new JObject();
            JArray itemlist = new JArray();
            // 아이템 저장 시작
            List<string> items = new List<string>() { "Heal50P" };
            foreach (var item in items)
            {
                itemlist.Add(item);
            }
            // 값 저장
            account.Add("ID", id);
            account.Add("Name", name);
            account.Add("Level", 1);
            account.Add("gold", 0);
            account.Add("RightHand", null);
            account.Add("LeftHand", null);
            account.Add("Armor", null);
            account.Add("Item", itemlist);
            // 경로 설정
            DirectoryInfo domain = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string fileName = $"{id}.json";
            string directory = domain.FullName + @"\" + fileName;
            // 데이터 파일 생성 (프로젝트 이름\bin\Debug\net6.0 경로에 저장)
            File.WriteAllText(directory, account.ToString());
            // 이름 리턴
            return name; 
        }

        // 아이디 입력
        Console.WriteLine("저장된 기록이 없습니다. ID를 입력해주세요.");
        string Pname = Console.ReadLine();
        Console.WriteLine("정상적으로 등록되었습니다.");
        Thread.Sleep(2000);
        Console.Clear();

        // 프롤로그
        Console.WriteLine("어느날, 하늘에서 떨어진 입방체 형태의 거대하고 검은 구조물.");
        Console.WriteLine("그 구조물은 떨어지면서 수많은 생명을 앗아갔다.");
        Console.WriteLine("뿐만 아니라 그 구조물의 입구로 추정되는 곳에서부터 괴물들이 나오기 시작했다.");
        Console.WriteLine("사람들은 이 괴물들에게 맞서기 위해 검은 구조물로 향했다.");
        Console.WriteLine("이 검은 구조물 내부로 들어가기 위해 많은 희생을 낳았지만 결국 사람들은 이 구조물 내부로 들어가는데 성공했다.");
        Console.WriteLine("안으로 들어가 수색을 했던 사람들은 이 구조물의 내부는 여러 개의 계층으로 나누어진 곳이란 걸 알아냈다.");
        Console.WriteLine("그리고 이 구조물을 만든 것이 「라비린스」라고 불리는 무언가였고 이곳에선 계속해서 괴물들이 나오고 있었다.");
        Console.WriteLine("이에 사람들은 이 곳을 「라비린스의 미궁」이라 칭하게 된다.");
        Console.WriteLine("당신은 라비린스의 미궁을 탐사하기 위해 온 모험가입니다.");
        Console.Write("당신의 이름은 무엇입니까? ");
        string Cname = Console.ReadLine();
        string PlayerName = Registration(Pname, Cname);

        // 초기 설정
        Warrior warrior = new Warrior(PlayerName);
        Goblin goblin = new Goblin("고블린");
        Dragon dragon = new Dragon("용");
        bool isBool;

        // 드롭 테이블 설정
        List<IItem> dropTable = new List<IItem> { new HealthPotion(), new StrengthPotion() };
        // 스테이지 설정
        Stage firstStage = new Stage(warrior, goblin, dropTable);
        Stage secondStage = new Stage(warrior, dragon, dropTable);
        List<Stage> StageList = new List<Stage> { firstStage, secondStage };

        // 게임 인트로
        Console.WriteLine("모험을 주로 하던 평범한 전사였던 당신은 던전을 발견하게 됩니다.");
        Console.WriteLine("던전에 입장하시겠습니까? (네 / 아니오)");
        Console.Write("당신: ");
        string start = Console.ReadLine();

        // 게임 시작 유무
        if (start == "네" || start == "sp" || start == "yes")
        {
            // 스테이지 시작
            foreach (Stage nextStage in StageList)
            {
                isBool = false;
                while (!isBool)
                {
                    Console.Clear();
                    isBool = nextStage.Start();
                }
            }
            // 모두 클리어
            Console.WriteLine("축하합니다! 당신은 모든 스테이지를 클리어하셨습니다!");
        }
        else
        {
            Console.WriteLine("던전이 두려워진 당신은 돌아가게 됩니다.");
        }

        
        
    }
    
    
}

