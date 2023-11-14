using System.Numerics;
using System.Reflection.Metadata;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Xml.Linq;

// 캐릭터 인터페이스 설정
public interface ICharacter
{
    string Name { get; }
    int Health { get; }
    int Attack { get; }
    int AD { get; }
    public int bonusAD { get; set; }
    public int bonusAC { get; set; }
    public int HP { get; set; }
    public int AC { get; set; }
    public int Level { get; set; }
    public int Gold { get; set; }
    bool IsDead { get; }
    void TakeDamage(int damage);
}

// 전사 클래스 설정
public class Warrior : ICharacter
{
    // 기본 스테이터스
    public string Name { get; }
    public int Health { get; set; }
    public int Attack { get; set; }
    public int bonusAD { get; set; }
    public int bonusAC { get; set; }
    public int HP { get; set; }
    public int AC { get; set; }
    public int Level { get; set; }
    public int Gold { get; set; }

    // 전투불능 체크
    public bool IsDead => HP <= 0;
    // 초기 설정
    public Warrior(string name, int lv, int gold)
    {
        Name = name;
        Health = 100;
        Level = lv;
        Gold = gold;
        Attack = 10;
        HP = Health + (Level - 1) * 10;
        AC = 5;
        bonusAD = 0;
        bonusAC = 0;
    }
    // 추가 스테이터스
    public int AD => new Random().Next((int)((Attack + (Level - 1) * 2) * 0.5 + bonusAD), (Attack + (Level - 1) * 2 + bonusAD));
    // 피해 교환
    public void TakeDamage(int damage)
    {
        int dps = damage - (AC + (Level - 1) + bonusAC);
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
// 사용자 데이터 클래스 설정
public class Member
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Job { get; set; }
    public int Level { get; set; }
    public int Gold { get; set; }
    public string RightHand { get; set; }
    public string LeftHand { get; set; }
    public string Armor { get; set; } 
    public int[] Item { get; set; }
}

// 몬스터 클래스 설정
public class Monster : ICharacter
{
    // 값이 변하지 않는 기본 스테이터스
    public string Name { get; }
    public int Health { get; }
    public int Attack { get; }
    public int bonusAD { get; set; }
    public int bonusAC { get; set; }
    public int HP { get; set; }
    public int AC { get; set; }
    public int Level { get; set; }
    public int Gold { get; set; }
    // 전투불능 체크
    public bool IsDead => HP <= 0;
    // 초기 설정
    public Monster(string name, int health, int attack, int ac, int gold)
    {
        Name = name;
        Health = health;
        Attack = attack;
        HP = Health;
        AC = ac;
        bonusAD = 0;
        bonusAC = 0;
        Level = 1;
        Gold = gold;
        
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
        int dps = damage - (AC + (Level - 1) + bonusAC);
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
    //고블린의 이름, 체력 50, 공격력 10, 방어력 0, 드랍 골드 2000 { } 안은 사용하지 않음.
    public Goblin(string name) : base(name, 50, 10, 0, 2000) { }
}

// 고블린 클래스 생성
public class Dragon : Monster
{
    //드래곤의 이름, 체력 150, 공격력 20, 방어력 5, 드랍 골드 5000 { } 안은 사용하지 않음.
    public Dragon(string name) : base(name, 150, 20, 5, 5000) { }
}

// 아이템 인터페이스 설정
public interface IItem
{
    public string ItemType { get; }
    public string Name { get; }
    public int Number { get; }
    public int HPValue { get; }
    public int ACValue { get; }
    public int AttackValue { get; }
    public int EquipInfo { get; }
    public string Info { get; }

    // 워리어가 아이템을 사용할 시
    public void Use(Warrior warrior);
}
// 장비
// 한손검 설정
public class Sword : IItem
{
    public string ItemType => "Sword";
    public int Number { get; }
    public string Name { get; }
    public int HPValue => 0;
    public int ACValue => 0;
    public int AttackValue { get; }
    public int EquipInfo { get; set; }
    public string Info { get; }

    public Sword(int number, string itemname, int value, string iteminfo)
    {
        Name = itemname;
        AttackValue = value;
        Info = iteminfo;
        EquipInfo = 1;
        Number = number;
    }
        // 워리어가 아이템을 사용할 시
        public void Use(Warrior warrior)
    {
        if(EquipInfo == 1)
        {
            EquipInfo = 2;
            warrior.bonusAD += AttackValue;

        }
        else if(EquipInfo == 2)
        { 
            EquipInfo = 1;
            warrior.bonusAD -= AttackValue;
        }
    }
}
public class shotSword : Sword
{
    //아이템 고유번호, 이름, 공격력, 장비 설명 { } 안은 사용하지 않음.
    public shotSword() : base(11110101, "쇼트스워드", 2, "흔하디 흔한 검들 중 하나다.") { }
}
// 방패
// 갑옷

// 소모품
// 체력 회복 물약 설정
public class HealthPotion : IItem
{
    public string ItemType => "Potion";
    public string Name => "체력 증가 물약";
    public int Number => 11412101;
    public int HPValue => 0;
    public int ACValue => 0;
    public int PotionValue => 100;
    public int AttackValue => 0;
    public int EquipInfo => 0;
    public string Info => "최대 체력이 100 증가하는 물약이다.";
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
    public string ItemType => "Potion";
    public string Name { get; }
    public int Number { get; }
    public int HPValue => 0;
    public int ACValue => 0;
    public int PotionValue { get; }
    public int AttackValue => 0;
    public int EquipInfo => 0;
    public string Info { get; set; }
    // 헬스포션 사용 시 효과 설정
    public HealingPotion(int num, int value)
    {
        Name = $"체력 회복 물약 {value}";
        PotionValue = value;
        Number = num;
        Info = $"체력을 {value} 만큼 회복시켜 주는 물약이다.";
    }
    public void Use(Warrior warrior)
    {
        Console.WriteLine($"{warrior.Name}의 체력이 {PotionValue} 만큼 회복합니다.");
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
    public string ItemType => "Potion";
    public string Name => "근육 뻠핑 물약";
    public int Number => 11412102;
    public int HPValue => 0;
    public int ACValue => 0;
    public int PotionValue => 0;
    public int AttackValue => 10; public int EquipInfo => 0;
    public string Info => "공격력이 10 증가하는 물약이다.";
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
    //아이템 고유번호, 힐 수치 { } 안은 사용하지 않음.
    public Heal50P() : base(11412103, 50) { }
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
        Death += BossEnd;
    }

    // 던전 시작
    public bool BossStart()
    {
        // 기본 세팅
        int turnCharacter = 0;
        int dps;
        bool IsBool = false;

        // 시작
        Console.WriteLine($"당신의 능력치는 체력 {warrior.Health}, 공격력 {warrior.Attack+(warrior.Level-1)*2+warrior.bonusAD} 입니다.");
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
    public void BossEnd(ICharacter character)
    {
        // 죽은 게 적인지 자신인지 확인
        if (character is Monster)
        {
            // 적 격파 성공
            Console.WriteLine("적을 쓰러트려 이 지역을 정복했습니다! 보상을 선택해주세요!");
            int num = 0;
            foreach (var drop in dropTable)
            {
                num++;
                // 보상 안내
                Console.WriteLine($"{num}. {drop.Name}");
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
                // 몹 체력 초기화
                monster.HP = monster.Health;
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
// 미궁 난이도
public class MiroClass
{
    public string Name { get; }
    public int Classname { get; }
    public int Ac { get; }
    public int Gold { get; set; }

    public MiroClass(string name, int classname, int ac, int gold)
    {
        Name = name;
        Classname = classname;
        Ac = ac;
        Gold = gold;
    }

    public int Goldbust(Warrior warrior)
    {
        var bust = new Random().Next((warrior.Attack + warrior.Level - 1 + warrior.bonusAD), ((warrior.Attack + warrior.Level - 1 + warrior.bonusAD) * 2));
        var goldbust = 1 + bust * 0.01;
        var bustgold = Gold * goldbust;
        Gold = (int)bustgold;
        return Gold;
    }
}
public class miro
{

    public Warrior warrior;
    public MiroClass miroclass;
    public List<int> dropGold;
    public string name;

    public int ac;

    // 캐릭터 및 미궁 값 설정
    public miro(Warrior warrior, MiroClass miroclass)
    {
        this.warrior = warrior;
        this.miroclass = miroclass;
    }
    // 미궁 난이도 설정
    public void Mirostart()
    {
        Console.Clear();
        int AC = warrior.AC + warrior.bonusAC;
        if (AC >= miroclass.Ac && warrior.HP > 1)
        {
            MiroClear();
        }
        else
        {
            int per = new Random().Next(1, 101);
            if(per >= 61 && warrior.HP > 1)
            {
                MiroClear();
            } else
            {
                Console.WriteLine("던전 실패");
                Console.WriteLine("체력이 절반으로 감소합니다.");
                double damage = warrior.HP / 2;
                double warhp = warrior.HP - damage;
                warrior.HP = (int)warhp;
            }
        }
        Console.WriteLine();
        Console.WriteLine("0. 나가기");
        Console.WriteLine();
        Console.Write("당신: ");
        string input = Console.ReadLine();
        bool b = false;
        while (b == false)
        {
            if (input == "0")
            {
                b = true;
            }
            else
            {
                input = Console.ReadLine();
            }
        }
    }
    // 미궁 탐사 성공
    public void MiroClear()
    {
        int AC = warrior.AC + warrior.bonusAC;
        int damage1 = 0;
        int damage2 = 0;
        Console.WriteLine("던전 클리어");
        Console.WriteLine("축하합니다!");
        Console.WriteLine($"{miroclass.Name}을 클리어 하였습니다.");
        Console.WriteLine();
        Console.WriteLine("[탐사 결과]");
        Console.Write($"체력 {warrior.HP} -> ");
        // 랜덤 데미지 결정
        if (miroclass.Classname == 1)
        {
            damage1 = 20 - AC;
            if (damage1 < 1) damage1 = 1;
            damage2 = 35 - AC;
            if (damage2 < 1) damage2 = 1;
        }
        else if (miroclass.Classname == 2)
        {
            damage1 = 20 - AC;
            if (damage1 < 1) damage1 = 1;
            damage2 = 35 - AC;
            if (damage2 < 1) damage2 = 1;
        }
        else if (miroclass.Classname == 3)
        {
            damage1 = 20 - AC;
            if (damage1 < 1) damage1 = 1;
            damage2 = 35 - AC;
            if (damage2 < 1) damage2 = 1;
        }
        // 데미지
        int random = new Random().Next(damage1, damage2);
        warrior.HP -= random;
        if (warrior.HP < 1) warrior.HP = 1;
        Console.WriteLine($"{warrior.HP}");
        // 보상 결산
        Console.Write($"골드 {warrior.Gold} -> ");
        int plusgold = miroclass.Goldbust(warrior);
        warrior.Gold += plusgold;
        Console.WriteLine($"{warrior.Gold}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        // 경로 설정
        DirectoryInfo Domain = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"save");
        Member member;
        // save 폴더가 없다면 생성
        if (!Domain.Exists)
        {
            Domain.Create();
        }
        // save 파일 존재 유무 확인
        string[] domainNum = Directory.GetFiles(Domain.ToString(), "*.json");
        // 계정 등록 및 확인
        Member Registration(string id, string name, DirectoryInfo domain)
        {
            // 기본값 생성
            JObject account = new JObject();
            JArray itemlist = new JArray();
            Member memberPath;
            // 아이템 저장 시작
            List<int> items = new List<int>() { 11110101, 11412103 };
            foreach (var item in items)
            {
                itemlist.Add(item);
            }
            // 값 저장
            account.Add("ID", id);
            account.Add("Name", name);
            account.Add("Job", "전사");
            account.Add("Level", 1);
            account.Add("Gold", 1500);
            account.Add("RightHand", null);
            account.Add("LeftHand", null);
            account.Add("Armor", null);
            account.Add("Item", itemlist);
            
            // 파일 경로 설정
            string fileName = $"{id}.json";
            string directory = domain.FullName + @"\" + fileName;
            // 데이터 파일 생성 (프로젝트 이름\bin\Debug\net6.0 경로에 저장)
            File.WriteAllText(directory, account.ToString());
            string json = File.ReadAllText(directory);
            memberPath = JsonConvert.DeserializeObject<Member>(json);
            // 이름 리턴
            return memberPath; 
        }

        // 계정 로그인
        Member Login()
        {
            
            // 변수 지정
            FileInfo[] saves = Domain.GetFiles();
            List<string> saveId = new List<string> { };
            string savePath;
            string json;
            Member memberPath;

            // 시작
            Console.WriteLine("아이디를 선택해주세요.");
            Console.WriteLine();
            // 아이디 목록 검색
            for (int num = 0; num < saves.Length; num++)
            {
                // 검색된 아이디 불러오기
                savePath = saves[num].FullName;
                json = File.ReadAllText(savePath);
                memberPath = JsonConvert.DeserializeObject<Member>(json);
                saveId.Add(memberPath.ID);
                Console.Write($"{num + 1}. {memberPath.ID}   ");
            }
            // 아이디 선택
            Console.WriteLine();
            string input = Console.ReadLine();
            if(input == "") input = "0";
            int inputisint = int.Parse(input);
            bool b = false;
            while(b == false)
            {
                if(inputisint > 0 && inputisint <= saves.Length)
                {
                    b = true;
                } else
                {
                    Console.WriteLine("옳지 않은 번호입니다. 다시 써주세요.");
                    input = Console.ReadLine();
                    if (input == "") input = "0";
                    inputisint = int.Parse(input);
                }
            }
            // 아이디 다시 불러오기
            int select = int.Parse(input) - 1;
            savePath = saves[select].FullName;
            json = File.ReadAllText(savePath);
            memberPath = JsonConvert.DeserializeObject<Member>(json);
            // 아이디 확정 후 리턴
            Console.WriteLine($"{memberPath.Name} 을(를) 불러 옵니다.");
            return memberPath;
        }
        // 반복 선택지 출력
        int SelectInput(int min, int max) {
            while (true)
            {
                string input = Console.ReadLine();
                bool inputNum = int.TryParse(input, out var selected);
                if (inputNum)
                {
                    if((selected >= min && selected <= max) || selected == 0)
                    {
                        return selected;
                    }
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.");
                    Console.Write("당신: ");
                }
            }
        }

        

        // save 파일이 존재하지 않을 경우 Registration 출력, 존재할 경우 Login 출력
        if (domainNum.Length == 0)
        {
            // 아이디 입력
            Console.WriteLine("저장된 기록이 없습니다. ID를 입력해주세요.");
            string Pname = Console.ReadLine();
            Console.WriteLine("정상적으로 등록되었습니다.");
            Thread.Sleep(2000);
            Console.Clear();

            // 프롤로그
            Console.WriteLine("어느날, 하늘에서 떨어진 입방체 형태의 거대하고 검은 구조물.");
            Console.WriteLine("그 구조물은 떨어지면서 수많은 생명들의 터전을 빼앗아갔다.");
            Console.WriteLine("뿐만 아니라 그 구조물의 입구로 추정되는 곳에서부터 괴물들이 나오기 시작했고,");
            Console.WriteLine("사람들은 이 괴물들에게 맞서기 위해 검은 구조물로 향했다.");
            Console.WriteLine("이 검은 구조물 내부로 들어가기 위해 많은 희생을 낳았지만 결국 사람들은 이 구조물 내부로 들어가는데 성공했다.");
            Console.WriteLine("안으로 들어가 수색을 했던 사람들은 이 구조물의 내부는 여러 개의 계층으로 나누어진 곳이란 걸 알아냈다.");
            Console.WriteLine("그리고 이 구조물을 만든 것이 「라비린스」라고 불리는 무언가였고 이곳에선 계속해서 괴물들이 나오고 있었다.");
            Console.WriteLine("이에 사람들은 이 곳을 「라비린스의 미궁」이라 칭하게 된다.");
            Console.WriteLine("당신은 라비린스의 미궁을 탐사하기 위해 온 모험가입니다.");
            Console.Write("당신의 이름은 무엇입니까? ");
            string Cname = Console.ReadLine();
            member = Registration(Pname, Cname, Domain);
        } else
        {
            Console.WriteLine("저장된 기록이 있습니다.");
            member = Login();
        }

        // 초기 설정
        Warrior warrior = new Warrior(member.Name, member.Level, member.Gold);
        Goblin goblin = new Goblin("고블린로드");
        Dragon dragon = new Dragon("용");
        List<IItem> inven = new List<IItem> { };
        MiroClass miroEasy = new MiroClass("1층 탐사(쉬움)", 1, 5, 1000);
        MiroClass miroNormal = new MiroClass("2층 탐사(보통)", 2, 11, 1700);
        MiroClass miroHard = new MiroClass("3층 탐사(어려움)", 3, 17, 2500);
        List<MiroClass> miroList = new List<MiroClass> { miroEasy, miroNormal, miroHard };
        bool isBool;

        // 인벤토리 설정
        foreach(int i in member.Item)
        {
            if(i == 11110101)
            {
                inven.Add(new shotSword());
            } else if(i == 11412103)
            {
                inven.Add(new Heal50P());
            }
        }
        // 드롭 테이블 설정
        List<IItem> dropTable = new List<IItem> { new HealthPotion(), new StrengthPotion() };
        // 스테이지 설정
        Stage firstBoss = new Stage(warrior, goblin, dropTable);
        Stage secondBoss = new Stage(warrior, dragon, dropTable);
        List<Stage> StageList = new List<Stage> { firstBoss, secondBoss };

        // 내정보 구현

        void MyInfo()
        {
            Console.Clear();

            Console.WriteLine("상태보기");
            Console.WriteLine("캐릭터의 정보를 표시합니다.");
            Console.WriteLine();
            Console.WriteLine($"Lv.{member.Level}");
            Console.WriteLine($"{member.Name}({member.Job})");
            Console.Write($"공격력 :{warrior.Attack} ");
            if(warrior.bonusAD > 0) { Console.WriteLine($"(+{warrior.bonusAD})"); } else { Console.WriteLine(); }
            Console.Write($"방어력 : {warrior.AC} ");
            if (warrior.bonusAC > 0) { Console.WriteLine($"(+{warrior.bonusAC})"); } else { Console.WriteLine(); }
            Console.WriteLine($"체력 : {warrior.HP}");
            Console.WriteLine($"Gold : {member.Gold} G");
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.Write("당신: ");

                int input = SelectInput(0, 0);
            switch (input)
            {
                case 0:
                    break;
            }
        }

        // 인벤토리 구현
        void Inventory()
        {
            Console.Clear();
            int num = 0;
            Console.WriteLine("인벤토리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            // 인벤토리에 소지 아이템 넣기
            foreach(var item in inven)
            {
                num++;
                Console.Write($"- {num} ");
                if(item.EquipInfo == 2) Console.Write("[E]");
                Console.Write($"{item.Name} | ");
                if (item.AttackValue != 0)
                {
                    Console.Write($"공격력 +{item.AttackValue} | ");
                }
                if (item.ACValue != 0)
                {
                    Console.Write($"방어력 +{item.ACValue} | ");
                }
                if (item.HPValue != 0)
                {
                    Console.Write($"회복력 +{item.HPValue} | ");
                }
                Console.WriteLine($"{item.Info}");
            }
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.Write("당신: ");
            int input = SelectInput(1, num);
            bool Binput = false;
            while (!Binput)
            {

                if (input <= num && input >= 0)
                {
                    Binput = true;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.");
                    Console.Write("당신: ");
                    input = SelectInput(1, num);
                }
            }
             
            // 아이템 사용
            if (input != 0 && input >= 1 && input <= num)
            {
                Console.Clear();
                // 장비인지 포션인지 구분
                if (inven[input - 1].ItemType != "Potion")
                {
                    // 장비일 경우
                    if (inven[input - 1].EquipInfo == 1)
                    {
                        Console.WriteLine($"{inven[input - 1].Name}을(를) 장착하시겠습니까?");
                    }
                    else
                    {
                        Console.WriteLine($"{inven[input - 1].Name}을(를) 해제하시겠습니까?");
                    }
                    // 사용 유무
                    Console.WriteLine("1. 예");
                    Console.WriteLine("2. 아니오");
                    Console.Write("당신: ");
                    string inp = Console.ReadLine();
                    // 2 이외를 입력해도 돌아가게 만듦
                    if (inp == "1")
                    {

                        inven[input - 1].Use(warrior);
                        Inventory();
                    }
                    else { Inventory(); }
                }
                else
                {
                    // 장비가 아닐 경우
                    Console.WriteLine($"{inven[input - 1].Name}을(를) 사용하시겠습니까?");
                    // 사용 유무
                    Console.WriteLine("1. 예");
                    Console.WriteLine("2. 아니오");
                    Console.Write("당신: ");
                    string inp = Console.ReadLine();
                    // 2 이외를 입력해도 돌아가게 만듦
                    if (inp == "1")
                    {
                        foreach(int j in member.Item)
                        {
                            bool b;
                            if (j == inven[input - 1].Number) member.Item = Array.FindAll(member.Item, j => j == inven[input - 1].Number).ToArray();
                        }
                        inven[input - 1].Use(warrior);
                        
                        inven.Remove(inven[input - 1]);
                        Inventory();
                    }
                    else { Inventory(); }
                }
            }
            switch (input)
            {
                case 0:
                    break;
            }
        }
        // 던전 설정
        void Dungeon()
        {
            Console.Clear();
            // 기본 설정
            int gold = 0;
            int num = 0;
            // 정보
            Console.WriteLine("당신은 라비린스의 미궁을 탐사하기 시작합니다.");
            Console.WriteLine();
            foreach (MiroClass mironame in miroList)
            {
                    num++;
                Console.WriteLine($"{num}. {mironame.Name}   | 방어력 {mironame.Ac} 이상 권장");
            }
            Console.WriteLine();
            Console.WriteLine("0. 돌아가기");
            Console.WriteLine();
            Console.WriteLine("난이도를 선택해주세요.");
            Console.Write("당신: ");
            // 값 지정
            int input = SelectInput(1, num);
            bool Binput = false;
            while (!Binput)
            {
                if ((input <= num && input >= 0) || input == 0)
                {
                    Binput = true;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.");
                    Console.Write("당신: ");
                    input = SelectInput(1, num);
                }
            }
            
            if(input == 1)
            {
                miro selectmiro = new miro(warrior, miroEasy);
                selectmiro.Mirostart();
            }
            else if (input == 2)
            {
                miro selectmiro = new miro(warrior, miroNormal);
                selectmiro.Mirostart();
            }
            else if (input == 3)
            {
                miro selectmiro = new miro(warrior, miroHard);
                selectmiro.Mirostart();
            }
        }
        // 게임 저장 설정
        Member Save()
        {
            Console.Clear();
            // 변수 지정
            JObject account = new JObject();
            JArray itemlist = new JArray();
            member.Level = warrior.Level;
            member.Gold = warrior.Gold;
            // 파일 경로 설정
            string fileName = $"{member.ID}.json";
            string directory = Domain.FullName + @"\" + fileName;
            // 시작
            Console.WriteLine("저장하기");
            Console.WriteLine("자신의 캐릭터를 저장합니다.");
            Console.WriteLine("단, 사용한 물약의 효과는 저장되지 않습니다.");
            Console.WriteLine("저장하시겠습니까?");
            Console.WriteLine();
            Console.WriteLine("1. 네");
            Console.WriteLine("0. 아니오");
            Console.WriteLine();
            int input = SelectInput(1, 1);
            if(input == 1)
            {
                // 아이템 저장 시작
                foreach (var item in member.Item)
                {
                    itemlist.Add(item);
                }
                // 값 저장
                account.Add("ID", member.ID);
                account.Add("Name", member.Name);
                account.Add("Job", member.Job);
                account.Add("Level", member.Level);
                account.Add("Gold", member.Gold);
                account.Add("RightHand", member.RightHand);
                account.Add("LeftHand", member.LeftHand);
                account.Add("Armor", member.Armor);
                account.Add("Item", itemlist);

                // 데이터 덮어쓰기 (프로젝트 이름\bin\Debug\net6.0 경로에 저장)
                File.Delete(directory);
                File.WriteAllText(directory, account.ToString());
            }
            
            string json = File.ReadAllText(directory);
            Member memberPath = JsonConvert.DeserializeObject<Member>(json);
            // 이름 리턴
            return memberPath;
        }
        // 게임 시작
        bool game = true;
        int start;
        while(game)
        {
            Console.Clear();
            Console.WriteLine("라비린스의 미궁에 오신 것을 환영합니다.");
            Console.WriteLine("미궁을 공략하기 전에 할 행동을 골라주세요.");
            Console.WriteLine();
            Console.WriteLine("1. 상태보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 미궁 탐사가기");
            Console.WriteLine("4. 미궁 보스방 탐사하기");
            Console.WriteLine("5. 저장하기");
            Console.WriteLine();
            Console.WriteLine("0. 종료");
            Console.WriteLine();
            Console.Write("당신: ");
            start = SelectInput(1, 5);

            // 스테이지 시작 유무
            if (start == 1)
            {
                MyInfo();
            }
            else if (start == 2)
            {
                Inventory();
            }
            else if (start == 3)
            {
                Dungeon();
            }
            else if (start == 4)
            {
                // 보스 챌린지 스테이지 시작
                foreach (Stage nextStage in StageList)
                {
                    Console.Clear();
                    isBool = nextStage.BossStart();
                }
                // 모두 클리어
                Console.WriteLine("축하합니다! 당신은 모든 보스를 쓰러트리셨습니다!");
            }
            else if (start == 5)
            {
                member = Save();
            }
            else if (start == 0)
            {
                game = false;
                Console.WriteLine("당신은 이만 짐을 싸고 돌아가게 됩니다.");
            } else
            {
                if (start != 10)
                {
                    game = false;
                }
            }
        }
    }
}

