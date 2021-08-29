
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyWork.Scripts.propagation
{
    public class ArrayMethods
    {
        public static void Fill2D<T>( T[,] array, T value)
        {
            int dimX = array.GetLength(0);
            int dimY = array.GetLength(1);
            
            for(int i = 0; i < dimX; i++) {
                
                for (int j = 0; j < dimY; j++)
                {
                    array[i, j] = value;
                }
            }
        }
        public static bool WithinBoundaries2D<T>(T[,] array, int x, int y)
        {
            return ((0 <= x) & (x < array.GetLength(0))) & ((0 <= y ) & (y < array.GetLength(1)));
        }
    }
    public enum TreeCompetition
    {
        Win = 1,
        Draw = 0,
        Loss = -1
    }

    public enum TreeStatus
    {
        Reproducing = 1,
        Alive = 0,
        Dead = -1 
    }

    public struct CheckRange
    {
        public int Start;
        public int End;
    }

    public class Tree
    {
        private static int deathAge = 60;
        private static int matureAge = 10;
        // TODO it should be possible to change these in unity 
        private static float startRFactor = 0.51f;
        
        public static float baseRadius; // THis should definitely be changeable 
        public static float maxRFactor;
        
        private static float deathFactor = 0.99f;

        private static float growFactor = 0.15f;
        
        // Variable in the object 
        private Vector2 pos;
        private float r;
        private int age;

        private bool alive = true;
        private bool enoughResources = true;
        private int yearsWithNoGrowth = 0;

        public Tree(float x, float y)
        {
            this.pos = new Vector2(x, y);
            this.r = startRFactor;
            this.age = 0;
        }

        public int YearsWithNoGrowth
        {
            get => yearsWithNoGrowth;
            set => yearsWithNoGrowth = value;
        }

        public bool EnoughResources
        {
            get => enoughResources;
            set => enoughResources = value;
        }

        public bool Alive
        {
            get => alive;
            set => alive = value;
        }

        public int Age
        {
            get => age;
            set => age = value;
        }

        public float R
        {
            get => r;
            set => r = value;
        }

        public Vector2 Pos
        {
            get => pos;
            set => pos = value;
        }

        public static float BaseRadius
        {
            get => baseRadius;
            set => baseRadius = value;
        }

        public static float GrowFactor
        {
            get => growFactor;
            set => growFactor = value;
        }

        public static float DeathFactor
        {
            get => deathFactor;
            set => deathFactor = value;
        }

        public static float MAXRFactor
        {
            get => maxRFactor;
            set => maxRFactor = value;
        }

        public static float StartRFactor
        {
            get => startRFactor;
            set => startRFactor = value;
        }

        public static int MatureAge
        {
            get => matureAge;
            set => matureAge = value;
        }

        public static int DeathAge
        {
            get => deathAge;
            set => deathAge = value;
        }

        public override string ToString()
        {
            return "TreeObject-Alive:" + this.alive + " x:" + this.pos.x + " y:" + this.pos.y + " Radius:" + this.r +
                   " Age:" + this.age; 
        }
        
        public TreeCompetition CheckCompetingTree(Tree other)
        {
            // I think you would want a enum type here. 
            float thisRealradius = this.r * baseRadius;
            float otherRealRadius = other.r * baseRadius;

            Vector2 dirVector = (other.pos - this.pos);
            float distance = dirVector.magnitude;

            // First we check if the tree are in each other sphere

            if (distance >= (otherRealRadius + thisRealradius) * 0.9)
            {
                // The disk are too far from each other 
                return TreeCompetition.Draw;
            }
            else if (thisRealradius > 1.1 * otherRealRadius)
            {
                return TreeCompetition.Win;
            }
            else if (thisRealradius * 1.1 < otherRealRadius)
            {
                return TreeCompetition.Loss;
            }
            else
            {
                if (this.age > other.age)
                {
                    return TreeCompetition.Win;
                }
                else if (this.age < other.age)
                {
                    return TreeCompetition.Loss;
                }
                else
                {
                    if (Random.value <= 0.5)
                    {
                        return TreeCompetition.Win;
                    }
                    else
                    {
                        return TreeCompetition.Loss;
                    }
                }
            }
            
            // OLD 
            if (distance < thisRealradius && otherRealRadius * 1.2 < thisRealradius)
            {
                // This tree is bigger then the other tree
                return TreeCompetition.Win;
            }
            else if (distance > otherRealRadius + thisRealradius)
            {
                // No collosion between the disk. So nothing happens

                return TreeCompetition.Draw;
            }
            else
            { 
                // THE disk collide 

                if (thisRealradius > 1.2 * otherRealRadius)
                {
                    // This tree is bigger then the other so it winns the fight
                    return TreeCompetition.Win;
                }
                else if (distance > thisRealradius*0.8)
                {
                    // The disk are of equal size and close to each other, we roll a dice over which one wins
                    if (Random.value < 0.5f)
                    {
                        return TreeCompetition.Win;
                    }
                    else
                    {
                        return TreeCompetition.Loss;
                    }
                }
                else
                {
                    // The disk are of equalize and far from each other so they can still grow. 
                    return TreeCompetition.Draw;
                }

            }
            
        }
        public void Grow()
        {
            this.yearsWithNoGrowth = 0;
            this.r = Mathf.Min(this.r + growFactor, maxRFactor);
        }
        
        public bool CheckIfDead()
        {  // TODO Maybe another methods should call on this and sett alive to the result instead  
            if (this.yearsWithNoGrowth >= 1)
            { // The tree has no growth in 3 years or more need. The tree dies 
                return true;
            }
            else if (this.age >= deathAge)
            {
                // The tree is past death Age so now there's a probability of it dying 

                float deathLimit = Mathf.Pow(deathFactor, age - deathAge);

                float outcome = Random.value;

                return outcome < deathLimit;
            }
            else
            {  // The tree passes all requirements so it is not dead
                return false;
            }
        }

        public TreeStatus AddYear()
        {
            if (this.enoughResources)
            {
                this.Grow();
            }
            else
            {
                this.yearsWithNoGrowth += 1;
            }

            if (this.CheckIfDead())
            {
                this.alive = false;
                return TreeStatus.Dead;
            }
            else
            {
                this.age += 1;
                return (this.age>= matureAge && this.r>= 0.75) ? TreeStatus.Reproducing : TreeStatus.Alive ;
            }
        }
    }

    class Forest
    {
    /*
    Succession:
    In each simulation iteration, every tree ages (and grows) until it reaches a mature age.
    Once a tree reaches a certain age, it dies and is culled from the population

    Plant propagation:
    Once trees have reached a mature age, they can reproduce by sowing seeds locally to their position.

    Self-thinning:
    If a tree is growing close to another tree, then the oldest (and largest) tree will out grow the other, thereby
    killing it and culling it from the environment. This is an approximation of asymmetric plant competition.
    */
    private int nStartTrees = 10;
    private int treeSpreadFactor = 20;

    private int nIter;
    private float mapWidth;
    private float mapHeight;
    
    private float cellSize;

    private int[,] grid;
    private CheckRange cRange;
    

    private List<Tree> allTrees;
    private List<Tree> activeTrees;
    private List<Tree> newTrees;
    
    public Forest(float mapWidth, float mapHeight, float baseR, float maxRFactor)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;

        cRange = new CheckRange();
        
        cRange.Start = - Mathf.CeilToInt(maxRFactor) - 1 ;//2 +3 ;
        cRange.End = Mathf.CeilToInt(maxRFactor) + 2;

        cellSize = baseR / Mathf.Sqrt(2);
        
        grid = new int[Mathf.CeilToInt(mapHeight / cellSize) , Mathf.CeilToInt(mapWidth / cellSize)];
        // Swapped x,y to y,x  
        
        BlueNoise_Scripts.ArrayMethods.Fill2D(grid, -1);
        
        allTrees = new List<Tree>();
        activeTrees = new List<Tree>();
        newTrees = new List<Tree>();
        
        Tree.baseRadius = baseR;
        Tree.maxRFactor = maxRFactor;
        CreateStartTrees();
    }
    private int GETCellNumb(float value)
    {
        return Mathf.FloorToInt(value / cellSize);
    }
    private void CreateStartTrees()
    {
        for (int i = 0; i < nStartTrees; i++)
        {
            float rdX = Random.Range(0, (mapWidth - 1));
            float rdY = Random.Range(0, (mapHeight - 1));

            int gridX = GETCellNumb(rdX);
            int gridY = GETCellNumb(rdY);

            if (grid[gridY, gridX] == -1)
            {
                Tree newTree = new Tree(rdX, rdY);
                allTrees.Add(newTree);
                activeTrees.Add(newTree);
                grid[gridY, gridX] = allTrees.Count - 1;
            }
        }
    }

    private void CheckGrowth(Tree tree)
    {
        if (!tree.EnoughResources)
        {
            return;
        }

        int gridX = GETCellNumb(tree.Pos.x);
        int gridY = GETCellNumb(tree.Pos.y);

        for (int yStep = cRange.Start; yStep < cRange.End; yStep++)
        { // TODO THIS IS NOT DONE
            for (int xStep = cRange.Start; xStep < cRange.End; xStep++)
            {
                if (!(yStep == 0 && xStep == 0))
                {
                    int y = gridY + yStep;
                    int x = gridX + xStep;
                
                    if (BlueNoise_Scripts.ArrayMethods.WithinBoundaries2D(grid, x, y))
                    {
                        int checkIndex = grid[y, x];

                        if (checkIndex > -1)
                        {
                            Tree competingTree = allTrees[checkIndex];
                            TreeCompetition result = tree.CheckCompetingTree(competingTree);
                        
                            if (result == TreeCompetition.Win )
                            {
                                //TODO I THINK THIS SHOULD KILL THE TREE
                                competingTree.EnoughResources = false;
                            }
                            else if (result == TreeCompetition.Loss)
                            {
                                tree.EnoughResources = false;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    private void TreeSpreading(Tree tree)
    {
        float r = tree.R*Tree.baseRadius;
        Vector2 baseTreePos = tree.Pos;

        for (int i = 0; i < treeSpreadFactor; i++)
        {
            float angle = Random.value * 2 * Mathf.PI;
            float annulusR = Random.Range(r, 2 * r);

            float seedX = baseTreePos.x + Mathf.Cos(angle) * annulusR;
            float seedY = baseTreePos.y + Mathf.Sin(angle) * annulusR;
            
            int gridX = GETCellNumb(seedX);
            int gridY = GETCellNumb(seedY);

            if (BlueNoise_Scripts.ArrayMethods.WithinBoundaries2D(this.grid, gridX, gridY))
            {
                Tree treeSeed = new Tree(seedX, seedY);

                if (this.grid[gridY, gridX] == -1)
                {
                    CheckGrowth(treeSeed);
                    if (treeSeed.EnoughResources)
                    {
                        newTrees.Add(treeSeed);
                        allTrees.Add(treeSeed);
                        grid[gridY, gridX] = allTrees.Count - 1;
                    }
                    else
                    {
                        //Debug.Log("Seed did not survive ");
                    } 
                    
                }
            }
        }
    }
    
    public void SimulateForrestGrowth(int nInter){

        for (int i = 0; i < nInter; i++)
        {
            foreach (Tree tree in activeTrees)
            {
                TreeStatus result = tree.AddYear();

                if (result == TreeStatus.Dead)
                {
                    int gridX = GETCellNumb(tree.Pos.x);
                    int gridY = GETCellNumb(tree.Pos.y);
                    grid[gridY, gridX] = -1;

                }else if (result == TreeStatus.Reproducing)
                {
                    TreeSpreading(tree);
                }
            }

            foreach (var newTree in newTrees) activeTrees.Add(newTree);
            // Adding all new trees to active trees
 
            foreach (var tree in activeTrees)
            {
                CheckGrowth(tree);
            }
            activeTrees = activeTrees.FindAll(t => t.Alive);
            newTrees =  new List<Tree>();
            
        }
    }

    public List<Vector3> GETPoints()
    {
        List<Vector3> resultingPoints = new List<Vector3>();
        foreach (Tree t in activeTrees)
        {
            if (t.Age > 20)
            {
                Vector2 pos = t.Pos;
                resultingPoints.Add(new Vector3(pos.x, 0.0f, pos.y));
            }
        }
        return resultingPoints;
    }
    }
}