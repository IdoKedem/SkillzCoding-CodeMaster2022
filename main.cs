using PenguinGame;
namespace MyBot 
{
 
    public class TutorialBot : ISkillzBot 
    {
        // Tool functions - used for creation of new variables, basic functions:
 
 
        // Returns list of exhibitions that are sent to a certain iceberg
        public PenguinGroup[] AllPGToIce(IceBuilding ice, PenguinGroup[] groupsList)
        {
            int counter = 0;
 
            for (int i = 0; i < groupsList.Length; i++)
            {
                if (groupsList[i].Destination == ice)
                    counter++;
            }
 
            PenguinGroup[] sentToIce = new PenguinGroup[counter];
 
            int j = 0;
            for (int i = 0; i < groupsList.Length; i++)
            {
                if (groupsList[i].Destination == ice)
                {
                    sentToIce[j] = groupsList[i];
                    j++;
                }
            }
 
            return sentToIce;
        }
 
        // Sorts an array by PenguinGroup distance from target. closest([0]) to farthest
        public PenguinGroup[] SortCloseToFar(Iceberg ice, PenguinGroup[] enemyGroups, PenguinGroup[] alliesGroups)
        {
            PenguinGroup[] alliesToIce = AllPGToIce(ice, alliesGroups);
            PenguinGroup[] enemyToIce = AllPGToIce(ice, enemyGroups);
 
            PenguinGroup[] sorted = new PenguinGroup[enemyToIce.Length + alliesToIce.Length];
 
            for(int i = 0; i < enemyToIce.Length; i++)
            {
                sorted[i] = enemyToIce[i];
            }
            for(int i = enemyToIce.Length; i < alliesToIce.Length + enemyToIce.Length; i++)
            {
                sorted[i] = alliesToIce[i - enemyToIce.Length];
            }
 
 
 
            for (int i = 0; i < sorted.Length - 1; i++)
            {            
               // Last i elements are already in place
               for (int j = 0; j < sorted.Length - i - 1; j++)
               {
                   int d1 = sorted[j].TurnsTillArrival;
                   int d2 = sorted[j + 1].TurnsTillArrival;
 
                    if (IsOnBridge(sorted[j]))
                        d1 = PGDistanceWithBridge(sorted[j]);
                    if (IsOnBridge(sorted[j+1]))
                        d2 = PGDistanceWithBridge(sorted[j+1]);
 
                   if (d1 > d2) 
                   {
                       PenguinGroup temp = sorted[j];
                       sorted[j] = sorted[j + 1];
                       sorted[j + 1] = temp;
                   }
               }
            }
 
            return sorted;
 
        }
 
        // Returns needed int penguin value needed to capture an Iceberg 
        public int NewGetNeeded(Iceberg ice, PenguinGroup[] enemyGroups, PenguinGroup[] alliesGroups, Iceberg myIceberg)
        {
            int point = myIceberg.GetTurnsTillArrival(ice);
            if (BridgeExists(myIceberg, ice))
                point = IcebergDWithBridge(myIceberg, ice);
 
            PenguinGroup[] groups = SortCloseToFar(ice, enemyGroups, alliesGroups);
 
            int needed = ice.PenguinAmount;
            int originalP = needed;
 
            int total = 0;
            int l = ice.PenguinsPerTurn; ///////////////////////
            int lastArrival = 0;
 
            bool neut = ice.Owner.Score == 0;
 
            if (neut && groups.Length == 0)
                return needed;
 
            if (ice.Owner.Id == 0 && !neut)
                needed = -needed;
 
            int i;
            System.Console.WriteLine("needed1 = " + needed);
 
            for (i = 0; i < groups.Length; i++) //Closer than point
            {
                int p = groups[i].PenguinAmount;
                int d = groups[i].TurnsTillArrival;
 
                if (IsOnBridge(groups[i]))
                    d = PGDistanceWithBridge(groups[i]);
 
                if (d > point-1) 
                    break;
 
                if (i != 0)
                    d = groups[i].TurnsTillArrival - groups[i-1].TurnsTillArrival;
 
                if (i < groups.Length - 1 && groups[i].TurnsTillArrival == groups[i+1].TurnsTillArrival && groups[i].Owner != groups[i+1].Owner)
                {
                    if(!neut)
                    {
                        if(needed < 0)
                        {
                            needed -= d * l;
                        }
                        if(needed > 0)
                        {
                            needed += d * l;
                        }
                    }
                    if (groups[i].Owner.Id == 0)
                    {
                        if (groups[i].PenguinAmount == groups[i+1].PenguinAmount )
                        {
                            if (neut)
                            {
                                if (needed < p)
                                {
                                    needed = 0;
                                    originalP = 0;
                                    neut = true;
                                }
                                else
                                    needed = needed - p; 
                            }
                        }
 
                        else
                        {
                            if(groups[i].PenguinAmount < groups[i+1].PenguinAmount )
                            {
                                int pDiff = groups[i+1].PenguinAmount - groups[i].PenguinAmount;
                                if (neut && groups[i+1].PenguinAmount >= needed)
                                    needed = pDiff ;
                                else
                                {
                                    if (neut)
                                        needed -= pDiff;
                                }
                            }
                            else 
                            {
                                int pDiff = groups[i].PenguinAmount - groups[i+1].PenguinAmount ;
                                needed = -pDiff;
                            }
                        }
                    }
 
                    else
                    {
                       if (groups[i].PenguinAmount == groups[i+1].PenguinAmount )
                        {
                            if (neut)
                            {
                                if (needed <= groups[i+1].PenguinAmount)
                                {
                                    needed = 0;
                                    originalP = 0;
                                    neut = true;
                                }
                                else
                                    needed= needed - groups[i+1].PenguinAmount; 
                            }
 
                        }
 
                        else
                        {
                            if(groups[i].PenguinAmount > groups[i+1].PenguinAmount )
                            {
                                int pDiff = groups[i].PenguinAmount - groups[i+1].PenguinAmount ;
                                if (neut && groups[i].PenguinAmount > needed)
                                    needed = pDiff ;
                                else
                                {
                                    if (neut)
                                        needed -= pDiff;
                                }
                            }
                            else 
                            {
                                int pDiff = groups[i+1].PenguinAmount - groups[i].PenguinAmount ;
                                needed = -pDiff;
                            }
                            neut = false;
                        } 
                    }
 
                    i++;
 
                    lastArrival = groups[i].TurnsTillArrival;
                }
                else 
                {
                    total += p;
 
                    if (needed == 0)
                    {
                        originalP = 0;
                        neut = true;
                    }
 
 
                    if (neut && total <= originalP) //Fighting on neutral 
                        needed -= p;
 
                    else
                    {
                        if (neut) //Capture moment
                        {
                            if (groups[i].Owner.Id == 0)
                            {
                                if (d == point-1)
                                    needed = p;
                                else
                                    needed = needed - p ;
                            }   
                            else
                            {
                                if (d == point-1)
                                    needed = p;
                                else
                                    needed = p - needed;
                            }
                            neut = false;
                        }
 
                        else //Captured
                        {
                            if (needed > 0)
                                needed += d * l;
                            else
                                needed -= d * l;
 
                            if (groups[i].Owner.Id == 0)
                                needed -= p;
                            else
                                needed += p;
                        }
                    }
                    lastArrival = groups[i].TurnsTillArrival;
                }
 
            }
 
            System.Console.WriteLine("needed2 = " + needed);
 
            int current;
 
            if (needed < 0)
            {
                needed -= (point - lastArrival) * l;
                current = needed;
            }
            else
            {
                if (needed > 0)
                    needed += (point - lastArrival) * l;
                current = 1;
            }
 
            bool firstBehind = true;
 
            System.Console.WriteLine("needed3 = " + needed);
 
            for (int j = i; j < groups.Length; j++)  //Farther than point
            {
                int p = groups[j].PenguinAmount;
                int d = groups[j].TurnsTillArrival;
 
                if (IsOnBridge(groups[j]))
                    d = PGDistanceWithBridge(groups[j]);
 
                if (groups[j].Owner.Id != 0 && groups.Length == 1)
                    return -1;
 
                if (firstBehind)
                {
                    firstBehind = false;
                    d = d - point;
                }
                else
                {
                    int d2 = groups[j-1].TurnsTillArrival;
 
                    if (IsOnBridge(groups[j-1]))
                        d2 = PGDistanceWithBridge(groups[j-1]);
 
                    d = d - d2;
 
                }
 
                if (groups[j].Owner.Id == 0)
                {
                    current += p + d * l;
                }
 
                else
                {
                    if (p >= current - d * l)
                    {
                        needed += p - (current + d * l);
                        current = 1;
                    }
                    else
                        current += d * l - p;
                }
            }
 
            System.Console.WriteLine("needed4 = " + needed);
 
            return needed;
        }
 
        //////////
        public int PenguinsFromBToA(Iceberg A, Iceberg B, PenguinGroup[] enemyP)
        {
            PenguinGroup[] penguinsfromBtoA = AllPGToIce(A, enemyP);
 
            int delivery = 0;
 
            for(int i = 0; i < penguinsfromBtoA.Length; i++)
            {
                if(penguinsfromBtoA[i].Source == B)
                {
                    delivery += penguinsfromBtoA[i].PenguinAmount;
                }
            }
 
            return delivery;
        }
 
        // Returns the amount of penguins on their way to a certain iceberg
        public int TotalExhibitions(IceBuilding ice, PenguinGroup[] groupsList)
        {
            PenguinGroup[] sentToIce = AllPGToIce(ice, groupsList);
            int transferred = 0;
 
            for (int i = 0; i < sentToIce.Length; i++){
                transferred += sentToIce[i].PenguinAmount;
            }
 
            return transferred;
        }
 
        // Returns the closest group to a certain iceberg
        public PenguinGroup ClosestGroup(IceBuilding ice, PenguinGroup[] pGroup)
        {
            PenguinGroup[] sentToIce = AllPGToIce(ice, pGroup);
 
            PenguinGroup closest = sentToIce[0];
            int d = closest.TurnsTillArrival;
 
            if(IsOnBridge(closest))
                d = PGDistanceWithBridge(closest);
 
            for(int i = 1; i < sentToIce.Length; i++)
            {
                int d2 = sentToIce[i].TurnsTillArrival;
 
                if(IsOnBridge(sentToIce[i]))
                    d2 = PGDistanceWithBridge(sentToIce[i]);
 
                if(d > d2)
                {
                    closest = sentToIce[i];
                    d = d2;
                }
            }
            return closest;
        } 
 
        // Sums all of the penguins on all icebergs in an array
        public int AllPenguins(Iceberg[] Icebergs)
        {
            int penguins = 0;
 
            for (int i = 0; i < Icebergs.Length; i++)
            {
                penguins += Icebergs[i].PenguinAmount;
            }
            return penguins;
        }
 
        // Returns diffrence of enemy exhibitions to friendly exhibitions
        public int GetDiff(Iceberg ice, PenguinGroup[] enemyP, PenguinGroup[] alliesP)
        {
            int sumEnemy = TotalExhibitions(ice, enemyP);
            int sumFriendly = TotalExhibitions(ice, alliesP);
 
            return sumEnemy - sumFriendly;
        }
 
        // Return sum of penguins in penguin group list
        public int SumOfPGroups(PenguinGroup[] pGroups)
        {
            int sum = 0;
 
            for(int i = 0; i < pGroups.Length;i++)
            {
                sum += pGroups[i].PenguinAmount;
            }
            return sum;
        }
 
        // Returns list of all penguin groups that are closer than a certain Penguin Group
        public PenguinGroup[] CloserExhibitionsThanGroup(Iceberg to, PenguinGroup group, PenguinGroup[] enemyP)
        {
            PenguinGroup[] pGroups = AllPGToIce(to, enemyP);
            int d1 = group.TurnsTillArrival;
 
            if(IsOnBridge(group))
                d1 = PGDistanceWithBridge(group);
 
            int counter = 0;
 
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d2 = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]))
                    d2 = PGDistanceWithBridge(pGroups[i]);
 
                if(d1 > d2)
                    counter++;
            }
 
            PenguinGroup[] closer = new PenguinGroup[counter];
 
            int j = 0;
 
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d2 = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]))
                    d2 = PGDistanceWithBridge(pGroups[i]);
 
                if(d1 > d2)
                {
                    closer[j] = pGroups[i];
                    j++;
                }
            }
            return closer;
        }
 
        // Returns a PenguinGroup that is farther than a certain penguin group but is closer than all the other farthest PenguinGroups from the same team
        public PenguinGroup ClosestFarthestThanGroup(Iceberg to, PenguinGroup pGroup, PenguinGroup[] pGroups)
        {
            int d1 = pGroup.TurnsTillArrival;
 
            if(IsOnBridge(pGroup))
                d1 = PGDistanceWithBridge(pGroup);
 
            PenguinGroup[] sentToIce = AllPGToIce(to, pGroups);
            PenguinGroup closestPG = sentToIce[0];
 
            int d2 = closestPG.TurnsTillArrival;
 
            if(IsOnBridge(closestPG))
                d2 = PGDistanceWithBridge(closestPG);
 
 
            for (int i = 1; i < sentToIce.Length; i++)
            {
                int d3 = sentToIce[i].TurnsTillArrival;
 
                if(IsOnBridge(sentToIce[i]))
                    d3 = PGDistanceWithBridge(sentToIce[i]);
 
                if (d3 > d1 && (d2 - d1) > (d3 - d1))
                {
                    closestPG = sentToIce[i];
                    d2 = d3;
                }
            }
            return closestPG;
        }
 
        // Returns list of all penguin groups that are closer than a certain Penguin Group
        public PenguinGroup[] CloserExhibitionsThanIceberg(Iceberg from, Iceberg to, PenguinGroup[] enemyP)
        {
            PenguinGroup[] pGroups = AllPGToIce(to, enemyP);
 
            int d1 = from.GetTurnsTillArrival(to);
 
            if(BridgeExists(from, to))
                d1 = IcebergDWithBridge(from, to);
 
            int counter = 0;
 
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d2 = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]))
                    d2 = PGDistanceWithBridge(pGroups[i]);
 
                if(d1 > d2)
                    counter++;
            }
 
            PenguinGroup[] closer = new PenguinGroup[counter];
 
            int j = 0;
 
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d2 = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]))
                    d2 = PGDistanceWithBridge(pGroups[i]);
 
                if(d1 > d2)
                {
                    closer[j] = pGroups[i];
                    j++;
                }
            }
            return closer;
        }
 
        // Returns all groups between two of given distance values NOT INCLUDING MIN AND MAX
        public PenguinGroup[] AllGroupsBetweenValues(PenguinGroup[] pGroups, int min, int max)
        {
            int counter = 0;
 
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]))
                    d = PGDistanceWithBridge(pGroups[i]);
 
                if(d > min && max > d )
                    counter++;
            }
 
            PenguinGroup[] between = new PenguinGroup[counter];
 
            int j = 0;
 
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]))
                    d = PGDistanceWithBridge(pGroups[i]);
 
                if(d > min && max > d)
                {
                    between[j] = pGroups[i];
                    j++;
                }
            }
            return between;
        }
 
        // Removes an iceberg from an array
        public Iceberg[] RemoveI(Iceberg[] icebergs, Iceberg iceberg)
        {
            Iceberg[] NewIcebergs = new Iceberg[icebergs.Length - 1];
 
            int j = 0;
            for (int i = 0; i < icebergs.Length; i++){
                if (icebergs[i].UniqueId != iceberg.UniqueId)
                    NewIcebergs[j] = icebergs[i];
                    j++;
            }
 
            return NewIcebergs;
        }
 
 
 
 
 
 
 
 
        // Bool functions - used for conditions:
 
 
        // Checks if an iceberg is neutral 
        public bool IsNeutral(IceBuilding ice, Iceberg[] Neuts)
        {
            for( int j = 0; j < Neuts.Length; j++)
            {
                if(ice.UniqueId == Neuts[j].UniqueId)
                    return true;
            }
            return false;
        }
 
        // Checks if we can win an iceberg
        public bool IsWon(Iceberg[] myIcebergs, Iceberg[] neuts, Iceberg target, PenguinGroup[] alliesP, PenguinGroup[] enemyP , int l)
        {
            if(AllPGToIce(target, alliesP).Length == 0)
                return false;
 
            int d1 = ClosestGroup(target, alliesP).TurnsTillArrival;
 
            if (IsOnBridge(ClosestGroup(target, alliesP)))
                d1 = PGDistanceWithBridge(ClosestGroup(target, alliesP));
 
            int p = target.PenguinAmount;
 
            bool neut = IsNeutral(target, neuts);
 
 
            if (AllPGToIce(target, enemyP).Length == 0)
            {
                if(neut)
                {
                    if (TotalExhibitions(target, alliesP) > p)
                        return true;
                    return false;    
                }
                else
                {
                    if (TotalExhibitions(target, alliesP) > p + d1 * l)
                        return true;
                    return false;
                }
            }
 
            int d2 = ClosestGroup(target, enemyP).TurnsTillArrival;
 
            if (IsOnBridge(ClosestGroup(target, enemyP)))
                d2 = PGDistanceWithBridge(ClosestGroup(target, enemyP));
 
 
            //int sumFriendly = TotalExhibitions(target, alliesP);
            int sumEnemy = TotalExhibitions(target, enemyP);
 
            PenguinGroup[] enemyCloser = CloserExhibitionsThanGroup(target, ClosestGroup(target, alliesP), enemyP);
 
            int sumOfCloser = SumOfPGroups(enemyCloser);
            int sumOfFarther = sumEnemy - sumOfCloser;
 
            PenguinGroup enemyClosestFarthest = ClosestFarthestThanGroup(target, ClosestGroup(target, alliesP), enemyP); // compared to out closest exhibition
            PenguinGroup[] friendlyCloser = CloserExhibitionsThanGroup(target, enemyClosestFarthest, enemyP); // compared to enemy closestFarthest
 
            int sumOfFriendlyCloser = SumOfPGroups(friendlyCloser);
 
            int d3 = enemyClosestFarthest.TurnsTillArrival;
 
            if (IsOnBridge(enemyClosestFarthest))
                d3 = PGDistanceWithBridge(enemyClosestFarthest);
 
            if(!neut) 
            {
                if(d1 > d2)
                {
                    if(sumOfFriendlyCloser > p + d1 * l + sumOfCloser)
                    {
                        if(sumOfFriendlyCloser - (p + d1 * l + sumOfCloser) + l * (d2 - d1) > sumOfFarther)
                            return true;
                        return false;
                    }
                    return false;
                }            
                else
                {
                    if(sumOfFriendlyCloser > p + d1 * l && sumOfFriendlyCloser - (p + d1 * l) + l * (d2 - d1) > sumEnemy)
                        return true;
                    return false;
                }
            }
            else
            {
                if(d1 > d2)
                {
                    if(p >= sumOfCloser)
                    {
                        if(sumOfFriendlyCloser > p - sumOfCloser && sumOfFriendlyCloser - (p - sumOfCloser) + l * (d3 - d1) > sumOfFarther)
                            return true;
                        return false;
                    }
                    else
                    {
                        if(sumOfFriendlyCloser > sumOfCloser - p + l * (d1 - d2))
                        {
                            if(sumOfFriendlyCloser - (sumOfCloser - p + l * (d1 - d2)) + l * (d3 - d1) > sumOfFarther)
                                return true;
                            return false;
                        }
                        return false;
                    }
                }
                else
                {
                    if(sumOfFriendlyCloser > p && sumOfFriendlyCloser - p + l * (d3 - d1) > sumOfFarther)
                        return true;
                    return false;
                }
            }
 
        }   
 
        // Checks if an iceberg is attacked
        public bool IsAttacked(Iceberg myIceberg, Iceberg AttackedI, PenguinGroup[] pG)
        {
            for (int i = 0; i < pG.Length; i++)
            {
                if (pG[i].Destination == AttackedI && pG[i].Source == myIceberg)
                    return true;
            }
            return false;
        }
 
        // Checks if there is AT LEAST ONE group farther than given d
        public bool IsFarther(PenguinGroup[] pGroups, int d1)
        {
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d2 = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]))
                    d2 = PGDistanceWithBridge(pGroups[i]);
 
                if(d2 > d1)
                    return true;
            }
            return false;
        }
 
        // Checks if there is AT LEAST ONE group closer than given d
        public bool IsCloser(PenguinGroup[] pGroups, int d1)
        {
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d2 = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]))
                    d2 = PGDistanceWithBridge(pGroups[i]);
 
                if(d2 < d1)
                    return true;
            }
            return false;
 
        }
 
        public bool CanSend(PenguinGroup[] alliesP, PenguinGroup[] enemyP, int exhibition, Iceberg ice)
        {
            if (ice.PenguinAmount - exhibition<=0)
                return false;
 
            if (AllPGToIce(ice, enemyP).Length==0)
                return true;
 
            PenguinGroup[] groups = SortCloseToFar(ice, alliesP, enemyP);
 
            int total = ice.PenguinAmount - exhibition;
            int l = ice.PenguinsPerTurn;
            int d = 0;
 
            for (int i=0; i<groups.Length ; i++ )
            {   
                if (i == 0)
                    d = groups[i].TurnsTillArrival;
                else 
                    d = groups[i].TurnsTillArrival - groups[i-1].TurnsTillArrival;
                total += d*l;
 
                if (groups[i].Owner.Id == 0)
                    total += groups[i].PenguinAmount;
                else 
                    total -= groups[i].PenguinAmount;
 
                if (total <= 0)
                    return false;
            }
            return true;
        }   
 
 
 
 
 
        // Location functions - used for finding certain icebergs:
 
 
        // Finds the best target
        public Iceberg GetTarget(Iceberg[] myIcebergs, Iceberg myIceberg, Iceberg[] eI, Iceberg[] Neuts, PenguinGroup[] myP, PenguinGroup[] enemyP)
        {
            Iceberg bestE = GetBestEnemy(myIceberg, eI, Neuts, myIcebergs, myP, enemyP);
 
 
            int p1 = bestE.PenguinAmount;
            int d1 = bestE.GetTurnsTillArrival(myIceberg);
            int l1 = bestE.PenguinsPerTurn;
            int c1 = bestE.UpgradeCost;
            int f1 = bestE.CostFactor;
            int diff1 = GetDiff(bestE, enemyP, myP);
 
            if(BridgeExists(myIceberg, bestE))
                d1 = IcebergDWithBridge(myIceberg, bestE);
 
            if (Neuts.Length > 0)
            {
                Iceberg bestN = FindBestN(myIceberg, Neuts, myIcebergs, myP, enemyP);
 
                int p2 = bestN.PenguinAmount;
                int d2 = bestN.GetTurnsTillArrival(myIceberg);
                int l2 = bestN.PenguinsPerTurn;
                int c2 = bestN.UpgradeCost;
                int f2 = bestN.CostFactor;
                int diff2 = GetDiff(bestN, enemyP, myP);
 
                if(d1 == 7 && eI[0] == bestE && d2 == 7 && l2 == 1 && p2 == 10 && myIceberg.PenguinsPerTurn == 1)
                    return bestE;
 
                if(BridgeExists(myIceberg, bestN))
                    d2 = IcebergDWithBridge(myIceberg, bestN);
 
                if(d2 > d1)
                {
                    if(l2 > l1)
                    {
                        if(((c1 + f1 * (l2 - l1 - 1)) - (l1 * (d2 - d1))) - ((diff1 + p1 + (d1 * l1) - (l1 * (d2 - d1))) - (p2 + diff2)) > 0)
                            return bestN;
                        return bestE;
                    }
                    else
                    {
                        if((p1 + (d1 * l1)) - (l1 * (d2 - d1)) + diff1 > p2 + diff2)
                            return bestN;
                        return bestE;
                    }
                }
                else
                {
                    if(l1 > l2)
                    {
                        if(((c2 + f2 * (l1 - l2 - 1)) - (l2 * (d1 - d2))) - (((p2 - l2 * (d1 - d2)) + diff2) - (p1 + d1 * l1 + diff1)) > 0)
                            return bestE;
                        return bestN;
                    }
 
                    if(p1 + (d1 * l1) + diff1 > p2 - (l2 * (d1 - d2)) + diff2)
                        return bestN;
                    return bestE;
                }
            }
            return bestE;
        }
 
        // Finds the best neutral target
        public Iceberg FindBestN(Iceberg myIceberg ,Iceberg[] Neuts, Iceberg[] myIcebergs, PenguinGroup[] myP, PenguinGroup[] enemyP)
        {
            Iceberg bestN = Neuts[0];
 
            int p1 = bestN.PenguinAmount;
            int d1 = bestN.GetTurnsTillArrival(myIceberg);
            int l1 = bestN.PenguinsPerTurn;
            int c1 = bestN.UpgradeCost;
            int f1 = bestN.CostFactor;
            int diff1 = GetDiff(bestN, enemyP, myP);
 
            if(BridgeExists(myIceberg, bestN))
                d1 = IcebergDWithBridge(myIceberg, bestN);
 
            for (int i = 1; i < Neuts.Length; i++){
 
                int p2 = Neuts[i].PenguinAmount;
                int d2 = Neuts[i].GetTurnsTillArrival(myIceberg);
                int l2 = Neuts[i].PenguinsPerTurn;
                int c2 = Neuts[i].UpgradeCost;
                int f2 = Neuts[i].CostFactor;
                int diff2 = GetDiff(Neuts[i], enemyP, myP);
 
                if(BridgeExists(myIceberg, Neuts[i]))
                    d2 = IcebergDWithBridge(myIceberg, Neuts[i]);
 
 
                if (d2 > d1)
                {
                    if(l2 > l1)
                    {
                        if(IsWon(myIcebergs, Neuts, bestN, myP, enemyP, l1) || !IsWon(myIcebergs, Neuts, Neuts[i], myP, enemyP, l2) && ((c1 + f1 * (l2 - l1 - 1)) - (l1 * (d2 - d1))) + ((p1 - (l1 * (d2 - d1)) + diff1) - (p2 + diff2)) > 0)
                        {
                            bestN = Neuts[i];
 
                            p1 = p2;
                            d1 = d2;
                            l1 = l2;
                            c1 = c2;
                            f1 = f2;   
                            diff1 = diff2;
                        }
                    }
                    else
                    {
                        if(IsWon(myIcebergs, Neuts, bestN, myP, enemyP, l1) || !IsWon(myIcebergs, Neuts, Neuts[i], myP, enemyP, l2) && p1 - (l1 * (d2 - d1)) + diff1 > p2 + diff2)        
                        {
                            bestN = Neuts[i];
 
                            p1 = p2;
                            d1 = d2;
                            l1 = l2;
                            c1 = c2;
                            f1 = f2;
                            diff1 = diff2;
                        }
                    }
                }
                else
                {
                    if(l1 > l2)
                    {
                        if(IsWon(myIcebergs, Neuts, bestN, myP, enemyP, l1) || !IsWon(myIcebergs, Neuts, Neuts[i], myP, enemyP, l2) && ((c2 + f2 * (l1 - l2 -1)) - (l2 * (d1 - d2))) + ((p2 - (l2 * (d1 - d2)) + diff2) - (p1 + diff1)) < 0 )
                        {
                            bestN = Neuts[i];
 
                            p1 = p2;
                            d1 = d2;
                            l1 = l2;
                            c1 = c2;
                            f1 = f2;
                            diff1 = diff2;
                        }
                    }
                    else
                    {
                        if (IsWon(myIcebergs, Neuts, bestN, myP, enemyP, l1) || !IsWon(myIcebergs, Neuts, Neuts[i], myP, enemyP, l2) && p2 - (l2 * (d1 - d2)) + diff2 < p1 + diff1)
                        {
                            bestN = Neuts[i];
 
                            p1 = p2;
                            d1 = d2;
                            l1 = l2;
                            c1 = c2;
                            f1 = f2;
                            diff1 = diff2;
                        }
                    }
                }
            }
            return bestN;
        }
 
        // Finds the best enemy target
        public Iceberg GetBestEnemy(Iceberg myIceberg, Iceberg[] eI, Iceberg[] Neuts, Iceberg[] myIcebergs, PenguinGroup[] myP, PenguinGroup[] enemyP)
        {
            Iceberg bestE = eI[0];
 
            int d1 = bestE.GetTurnsTillArrival(myIceberg);
            int l1 = bestE.PenguinsPerTurn;
            int p1 = bestE.PenguinAmount;
            int c1 = bestE.UpgradeCost;
            int f1 = bestE.CostFactor;
            int diff1 = GetDiff(bestE, enemyP, myP);
 
            if(BridgeExists(myIceberg, bestE))
                d1 = IcebergDWithBridge(myIceberg, bestE);
 
            for (int i = 1; i < eI.Length; i++)
            {
                int d2 = eI[i].GetTurnsTillArrival(myIceberg);
                int l2 = eI[i].PenguinsPerTurn;
                int p2 = eI[i].PenguinAmount;
                int c2 = eI[i].UpgradeCost;
                int f2 = eI[i].CostFactor;
                int diff2 = GetDiff(eI[i], enemyP, myP);
 
                if(BridgeExists(myIceberg, eI[i]))
                    d2 = IcebergDWithBridge(myIceberg, eI[i]);
 
                if(d2 > d1) 
                {
                    if(l2 > l1)
                    {
                        if(!IsWon(myIcebergs, Neuts, eI[i], myP, enemyP, eI[i].PenguinsPerTurn) && (c1 + f1 * (l2 - l1 - 1) - (l1 * (d2 - d1))) + ((p1 + d1 * l1 - (l1 * (d2 - d1)) + diff1) - (p2 + d2 * l2 + diff2)) > 0)
                        {
                            bestE = eI[i];
 
                            d1 = d2;
                            l1 = l2;
                            p1 = p2;
                            c1 = c2;
                            f1 = f2;
                            diff1 = diff2;
                        }
                    }
                    else
                    {
                        if(!IsWon(myIcebergs, Neuts, eI[i], myP, enemyP, eI[i].PenguinsPerTurn) && p2 + (d2 * l2) + diff2 < p1 + (d1 * l1) - (l1 * (d2 - d1)) + diff1)
                        {
                            bestE = eI[i];
 
                            d1 = d2;
                            l1 = l2;
                            p1 = p2;
                            c1 = c2;
                            f1 = f2;
                            diff1 = diff2;
 
                        }
                    }
                }
                else
                {
                    if(l1 > l2)
                    {
                        if(!IsWon(myIcebergs, Neuts, eI[i], myP, enemyP, eI[i].PenguinsPerTurn) && (c2 + f2 * (l1 - l2 - 1) - (l2 * (d1 - d2))) + ((p2 + d2 * l2 - (l2 * (d1 - d2)) - diff2) - (p1 + d1 * l1 - diff1)) < 0)
                        {
                            bestE = eI[i];
 
                            d1 = d2;
                            l1 = l2;
                            p1 = p2;
                            c1 = c2;
                            f1 = f2;
                            diff1 = diff2;
                        }
                    }
                    else
                    {
                        if (!IsWon(myIcebergs, Neuts, eI[i], myP, enemyP, eI[i].PenguinsPerTurn) && p2 + (d2 * l2) - l2 * (d1 - d2) - diff2 < p1 + (d1 * l1) - diff1)
                        {
                            bestE = eI[i];
 
                            d1 = d2;
                            l1 = l2;
                            p1 = p2;
                            c1 = c2;
                            f1 = f2;
                            diff1 = diff2;
                        }
                    }
                }
            }
            return bestE;
        }
 
        // Finds all possible ally defenders
        public Iceberg[] GetDefenders(Iceberg[] myI, Iceberg[] eI, PenguinGroup[] alliesP, PenguinGroup[] enemyP, Iceberg defended)
        {
            int counter = 0;
 
            for (int i = 0; i < myI.Length; i++)
            {
                if (myI[i] != defended)
                {
                    int d1 = myI[i].GetTurnsTillArrival(defended);
 
                    if(BridgeExists(myI[i], defended))
                        d1 = IcebergDWithBridge(myI[i], defended);
 
                    int needed = NewGetNeeded(defended, enemyP, alliesP, myI[i]);
 
                    if (CanSend(alliesP, enemyP, needed, myI[i]))
                        counter++;
                }
            }
 
            Iceberg[] defenders = new Iceberg[counter];
 
            int j = 0;
 
            for (int i = 0; i < myI.Length; i++)
            {
                if (myI[i] != defended)
                {
                    int d1 = myI[i].GetTurnsTillArrival(defended);
 
                    if(BridgeExists(myI[i], defended))
                        d1 = IcebergDWithBridge(myI[i], defended);
 
                    int needed = NewGetNeeded(defended, enemyP, alliesP, myI[i]);
 
                    if (CanSend(alliesP, enemyP, needed, myI[i]))
                    {
                        defenders[j] = myI[i];
                        j++;
                    }
                }
            }
            return defenders;
        }
 
 
 
 
 
 
 
 
        //Feature functions - used for dealing with the challenges:
 
 
        //Week 1 - Treasure
 
        // Defends the treasure
        public int TreasureDefense(IceBuilding myIceberg, Iceberg[] myIcebergs, PenguinGroup[] enemyP , PenguinGroup[] alliesP)
        {
            int j;
            Iceberg defender;
 
            if(myIceberg.UniqueId != myIcebergs[0].UniqueId)
            {
                defender = myIcebergs[0];
                j = 1;
            }
            else
            {
                defender = myIcebergs[1];
                j = 2;
            }
 
            int d = defender.GetTurnsTillArrival(myIceberg);
            int p = defender.PenguinAmount; 
 
            int d2, p2;
 
 
            for(int i = j; i < myIcebergs.Length; i++)
            {
                d2 = myIcebergs[i].GetTurnsTillArrival(myIceberg);
                p2 = myIcebergs[i].PenguinAmount;
 
                if(p2 > TotalExhibitions(myIceberg, enemyP))
                {
                    if(d > d2)
                        defender = myIcebergs[i];
                    else
                    {
                        if(d == d2 && p2 > p)
                            defender = myIcebergs [i];
                    }
                }
            }
 
            int sumEnemy = TotalExhibitions(myIceberg, enemyP);
 
            int defenderD = defender.GetTurnsTillArrival(myIceberg);
            int L = 0;
 
            if(defender.PenguinAmount >= TotalExhibitions(myIceberg, enemyP))
            {
                if((ClosestGroup(myIceberg, enemyP).TurnsTillArrival > defender.GetTurnsTillArrival(myIceberg) && CanSend(alliesP, enemyP, (sumEnemy - (myIceberg.PenguinAmount + (defenderD * L)) + 1), defender)))
                {
                    defender.SendPenguins(myIceberg, (sumEnemy - (myIceberg.PenguinAmount + (defenderD * L))) + 1);
                    return (sumEnemy - (myIceberg.PenguinAmount + (defenderD * L))) + 1;
                }
                else
                {
                    if ((CanSend(alliesP, enemyP, (sumEnemy - myIceberg.PenguinAmount) + (defenderD * L) + 1, defender)) && (sumEnemy - myIceberg.PenguinAmount) + (defenderD * L) + 1 > 0)
                    {
                        defender.SendPenguins(myIceberg, (sumEnemy - myIceberg.PenguinAmount) + (defenderD * L) + 1);
                        return (sumEnemy - myIceberg.PenguinAmount) + (defenderD * L) + 1;
                    }
                }
            }
            return 0;
        }
 
        // Runs the actions on the treasure
        public int TreasureFight(Iceberg[] myI, IceBuilding ozar, Iceberg myIceberg, Iceberg[] eIceberg, PenguinGroup[] eP, PenguinGroup[] alliesP, int TreasureForce)
        {
            int ozarP = ozar.PenguinAmount;
            int myIcebergP = myIceberg.PenguinAmount;
 
            int enemyPenguins = TotalExhibitions(ozar, eP);
            int alliesPenguins = TotalExhibitions(ozar, alliesP);
 
 
            if(ozar.Owner != myI[0].Owner)
            {
                if(ozar.Owner != eIceberg[0].Owner)
                {
                    if(enemyPenguins > ozarP && ClosestGroup(ozar, eP).TurnsTillArrival < myIceberg.GetTurnsTillArrival(ozar) && myIcebergP > enemyPenguins - ozarP +10 && alliesPenguins + TreasureForce < enemyPenguins - ozarP +10)
                    {
                        if(!IsBonusWon(ozar, alliesP, eP, TreasureForce) && !myIceberg.AlreadyActed && enemyPenguins - ozarP + 1 - alliesPenguins > 0 && CanSend(alliesP, eP, enemyPenguins - ozarP + 10 - alliesPenguins, myIceberg))
                        {
                            myIceberg.SendPenguins(ozar, enemyPenguins - ozarP + 10 - alliesPenguins);
                            return enemyPenguins - ozarP + 10 - alliesPenguins;
                        }
                    }
                }
                else
                {
                    if(CanSend(alliesP, eP, enemyPenguins + ozarP + 10 - alliesPenguins - TreasureForce , myIceberg) && !myIceberg.AlreadyActed && enemyPenguins + ozarP + 10 - alliesPenguins - TreasureForce > 0 )
                    {
                        myIceberg.SendPenguins(ozar, enemyPenguins + ozarP + 10 - alliesPenguins - TreasureForce);
                        return enemyPenguins + ozarP + 10 - alliesPenguins - TreasureForce;
                    }
                }
            }
            else
            {
                if(ozarP + alliesPenguins + TreasureForce < enemyPenguins && myI.Length > 1)
                    return TreasureDefense(ozar, myI, eP,alliesP);
            }
            return 0;
        }
 
        // Checks if we won the bonus
        public bool IsBonusWon(IceBuilding ozar, PenguinGroup[] alliesP, PenguinGroup[] enemyP, int TreasureForce)
        {
            if(AllPGToIce(ozar, alliesP).Length == 0)
                return false;
 
 
            int p = ozar.PenguinAmount;
 
            int sumFriendly = TotalExhibitions(ozar, alliesP) + TreasureForce;
            int sumEnemy = TotalExhibitions(ozar, enemyP);
 
 
            if(sumEnemy + p >= sumFriendly)
                return false;
            else
                return true;
        }
 
 
 
 
        //Week 2 = Bridges
 
        // Checks if a penguin group is on a bridge
        public bool IsOnBridge(PenguinGroup pGroup)
        {
            Iceberg source = (Iceberg) pGroup.Source;
            Bridge[] bridges = source.Bridges;
 
            for (int i = 0; i < bridges.Length; i++)
            {
                if (bridges[i].GetEdges()[0] == pGroup.Destination || bridges[i].GetEdges()[1] == pGroup.Destination)
                    return true;
            }
            return false;
        }
 
        // Returns the bridge that a given group is on
        public Bridge GetBridgeByGroup(PenguinGroup pGroup)
        {
            Iceberg source = (Iceberg) pGroup.Source;
            Bridge[] bridges = source.Bridges;
 
            for (int i = 0; i < bridges.Length; i++)
            {
                if (bridges[i].GetEdges()[0] == pGroup.Destination || bridges[i].GetEdges()[1] == pGroup.Destination)
                    return bridges[i];
            }
            return null;
        }
 
        // Returns the bridge which connects 2 Icebergs
        public Bridge GetBridgeByIcebergs(Iceberg from, IceBuilding to)
        {
            Bridge[] bridges = from.Bridges;
 
            for (int i = 0; i < bridges.Length; i++)
            {
                if (bridges[i].GetEdges()[0] == to || bridges[i].GetEdges()[1] == to)
                    return bridges[i];
            }
            return null;
        }
 
        // Returns the remaining distance of a group to its destination with a bridge
        public int PGDistanceWithBridge(PenguinGroup pGroup)
        {
            if(IsOnBridge(pGroup))
            {
                Bridge pBridge = GetBridgeByGroup(pGroup);
 
                if (pGroup.TurnsTillArrival < pBridge.Duration * pBridge.SpeedMultiplier)
                    return (int) (pGroup.TurnsTillArrival / pBridge.SpeedMultiplier);
                else
                    return (int) (pBridge.Duration + pGroup.TurnsTillArrival - pBridge.Duration * pBridge.SpeedMultiplier);
            }
            else
            {
                Iceberg source = (Iceberg) pGroup.Source;
 
                if(pGroup.TurnsTillArrival < source.MaxBridgeDuration * source.BridgeSpeedMultiplier)
                    return (int) (pGroup.TurnsTillArrival/ source.BridgeSpeedMultiplier);
                else
                    return (int) (source.MaxBridgeDuration + pGroup.TurnsTillArrival - source.MaxBridgeDuration * source.BridgeSpeedMultiplier);
            }
        }
 
        // Checks whether a bridge exists between two icebergs
        public bool BridgeExists(Iceberg from, Iceberg to)
        {
            Bridge[] fromB = from.Bridges;
 
            for(int i = 0; i < fromB.Length; i++)
            {
                if (fromB[i].GetEdges()[0] == to || fromB[i].GetEdges()[1] == to)
                    return true;
            }
            return false;
        }
 
        // Returns the distance between 2 Icebergs with Bridge
        public int IcebergDWithBridge(Iceberg myIceberg, Iceberg attackedI)
        {
            double d = myIceberg.GetTurnsTillArrival(attackedI);
 
            int duration = myIceberg.MaxBridgeDuration;
            double multiplier = myIceberg.BridgeSpeedMultiplier;
 
            if(BridgeExists(myIceberg, attackedI))
            {
                Bridge bridge = GetBridgeByIcebergs(myIceberg, attackedI);
                duration = bridge.Duration;
            }
 
            if (d > duration * multiplier)
                d =  d - (duration * multiplier);
            else
                d /= multiplier;
 
            return (int)d;
        }
 
        // Checks if a bridge will help a group win the target
        public bool IsWonWithBridge(Iceberg[] myIcebergs, Iceberg[] neuts, Iceberg target, PenguinGroup[] alliesP, PenguinGroup[] enemyP , int l, PenguinGroup pGroup)
        {
            if(AllPGToIce(target, alliesP).Length == 0)
                return false;
 
            int d1 = ClosestGroup(target, alliesP).TurnsTillArrival;
 
            if (IsOnBridge(ClosestGroup(target, alliesP)) || ClosestGroup(target, alliesP) == pGroup)
                d1 = PGDistanceWithBridge(ClosestGroup(target, alliesP));
 
            int p = target.PenguinAmount;
 
            bool neut = IsNeutral(target, neuts);
 
 
            if (AllPGToIce(target, enemyP).Length == 0)
            {
                if(neut)
                {
                    if (TotalExhibitions(target, alliesP) > p)
                        return true;
                    return false;    
                }
                else
                {
                    if (TotalExhibitions(target, alliesP) > p + d1 * l)
                        return true;
                    return false;
                }
            }
 
            int d2 = ClosestGroup(target, enemyP).TurnsTillArrival;
 
            if (IsOnBridge(ClosestGroup(target, enemyP)))
                d2 = PGDistanceWithBridge(ClosestGroup(target, enemyP));
 
 
            //int sumFriendly = TotalExhibitions(target, alliesP);
            int sumEnemy = TotalExhibitions(target, enemyP);
 
            PenguinGroup[] enemyCloser = CloserExhibitionsThanGroupWB(target, ClosestGroup(target, alliesP), enemyP, pGroup);
 
            int sumOfCloser = SumOfPGroups(enemyCloser);
            int sumOfFarther = sumEnemy - sumOfCloser;
 
            PenguinGroup enemyClosestFarthest = ClosestFarthestThanGroupWB(target, ClosestGroup(target, alliesP), enemyP, pGroup); // compared to out closest exhibition
            PenguinGroup[] friendlyCloser = CloserExhibitionsThanGroupWB(target, enemyClosestFarthest, enemyP, pGroup); // compared to enemy closestFarthest
 
            int sumOfFriendlyCloser = SumOfPGroups(friendlyCloser);
 
            int d3 = enemyClosestFarthest.TurnsTillArrival;
 
            if (IsOnBridge(enemyClosestFarthest))
                d3 = PGDistanceWithBridge(enemyClosestFarthest);
 
            if(!neut) 
            {
                if(d1 > d2)
                {
                    if(sumOfFriendlyCloser > p + d1 * l + sumOfCloser)
                    {
                        if(sumOfFriendlyCloser - (p + d1 * l + sumOfCloser) + l * (d2 - d1) > sumOfFarther)
                            return true;
                        return false;
                    }
                    return false;
                }            
                else
                {
                    if(sumOfFriendlyCloser > p + d1 * l && sumOfFriendlyCloser - (p + d1 * l) + l * (d2 - d1) > sumEnemy)
                        return true;
                    return false;
                }
            }
            else
            {
                if(d1 > d2)
                {
                    if(p >= sumOfCloser)
                    {
                        if(sumOfFriendlyCloser > p - sumOfCloser && sumOfFriendlyCloser - (p - sumOfCloser) + l * (d3 - d1) > sumOfFarther)
                            return true;
                        return false;
                    }
                    else
                    {
                        if(sumOfFriendlyCloser > sumOfCloser - p + l * (d1 - d2))
                        {
                            if(sumOfFriendlyCloser - (sumOfCloser - p + l * (d1 - d2)) + l * (d3 - d1) > sumOfFarther)
                                return true;
                            return false;
                        }
                        return false;
                    }
                }
                else
                {
                    if(sumOfFriendlyCloser > p && sumOfFriendlyCloser - p + l * (d3 - d1) > sumOfFarther)
                        return true;
                    return false;
                }
            }
 
        } 
 
         // Returns list of all penguin groups that are closer than a certain Penguin Group (WB)
        public PenguinGroup[] CloserExhibitionsThanGroupWB(Iceberg to, PenguinGroup group, PenguinGroup[] enemyP, PenguinGroup CpGroup)
        {
            PenguinGroup[] pGroups = AllPGToIce(to, enemyP);
            int d1 = group.TurnsTillArrival;
 
            if(IsOnBridge(group) || group == CpGroup)
                d1 = PGDistanceWithBridge(group);
 
            int counter = 0;
 
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d2 = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]) || pGroups[i] == CpGroup)
                    d2 = PGDistanceWithBridge(pGroups[i]);
 
                if(d1 > d2)
                    counter++;
            }
 
            PenguinGroup[] closer = new PenguinGroup[counter];
 
            int j = 0;
 
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d2 = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]) || pGroups[i] == CpGroup)
                    d2 = PGDistanceWithBridge(pGroups[i]);
 
                if(d1 > d2)
                {
                    closer[j] = pGroups[i];
                    j++;
                }
            }
            return closer;
        }
 
        // Returns a PenguinGroup that is farther than a certain penguin group but is closer than all the other farthest PenguinGroups from the same team (WB)
        public PenguinGroup ClosestFarthestThanGroupWB(Iceberg to, PenguinGroup pGroup, PenguinGroup[] pGroups, PenguinGroup CpGroup)
        {
            int d1 = pGroup.TurnsTillArrival;
 
            if(IsOnBridge(pGroup) || pGroup == CpGroup)
                d1 = PGDistanceWithBridge(pGroup);
 
            PenguinGroup[] sentToIce = AllPGToIce(to, pGroups);
            PenguinGroup closestPG = sentToIce[0];
 
            int d2 = closestPG.TurnsTillArrival;
 
            if(IsOnBridge(closestPG) || closestPG == CpGroup)
                d2 = PGDistanceWithBridge(closestPG);
 
            for (int i = 1; i < sentToIce.Length; i++)
            {
                int d3 = sentToIce[i].TurnsTillArrival;
 
                if(IsOnBridge(sentToIce[i]) || sentToIce[i] == CpGroup)
                    d3 = PGDistanceWithBridge(sentToIce[i]);
 
                if (d3 > d1 && (d2 - d1) > (d3 - d1))
                {
                    closestPG = sentToIce[i];
                    d2 = d3;
                }
            }
            return closestPG;
        }
 
        // Returns list of all penguin groups that are closer than a certain Penguin Group (WB)
        public PenguinGroup[] CloserExhibitionsThanIcebergWB(Iceberg from, Iceberg to, PenguinGroup[] enemyP, PenguinGroup CpGroup)
        {
            PenguinGroup[] pGroups = AllPGToIce(to, enemyP);
 
            int d1 = from.GetTurnsTillArrival(to);
 
            if(BridgeExists(from, to))
                d1 = IcebergDWithBridge(from, to);
 
            int counter = 0;
 
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d2 = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]) || pGroups[i] == CpGroup)
                    d2 = PGDistanceWithBridge(pGroups[i]);
 
                if(d1 > d2)
                    counter++;
            }
 
            PenguinGroup[] closer = new PenguinGroup[counter];
 
            int j = 0;
 
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d2 = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]) || pGroups[i] == CpGroup)
                    d2 = PGDistanceWithBridge(pGroups[i]);
 
                if(d1 > d2)
                {
                    closer[j] = pGroups[i];
                    j++;
                }
            }
            return closer;
        }
 
        // Returns a PenguinGroup that is farther than a certain penguin group but is closer than all the other farthest PenguinGroups from the same team (WB)
        public PenguinGroup ClosestFarthestThanIcebergWB(Iceberg from, Iceberg to, PenguinGroup[] pGroups, PenguinGroup CpGroup)
        {
            int d1 = from.GetTurnsTillArrival(to);
 
            if(BridgeExists(from, to));
                d1 = IcebergDWithBridge(from, to);
 
            PenguinGroup[] sentToIce = AllPGToIce(to, pGroups);
 
            PenguinGroup closestPG = sentToIce[0];
            int d2 = closestPG.TurnsTillArrival;
 
            if(IsOnBridge(closestPG) || closestPG == CpGroup)
                d2 = PGDistanceWithBridge(closestPG);
 
            for (int i = 1; i < sentToIce.Length; i++)
            {
                int d3 = sentToIce[i].TurnsTillArrival;
 
                if(IsOnBridge(sentToIce[i]) || sentToIce[i] == CpGroup)
                    d3 = PGDistanceWithBridge(sentToIce[i]);
 
                if (d3 > d1 && (d2 - d1) > (d3 - d1))
                {
                    closestPG = sentToIce[i];
                    d2 = d3;
                }
            }
            return closestPG; 
        }
 
        // Checks if there is AT LEAST ONE group farther than given d (WB)
        public bool IsFartherWB(PenguinGroup[] pGroups, int d1, PenguinGroup CpGroup)
        {
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d2 = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]) || pGroups[i] == CpGroup)
                    d2 = PGDistanceWithBridge(pGroups[i]);
 
                if(d2 > d1)
                    return true;
            }
            return false;
        }
 
        // Checks if there is AT LEAST ONE group closer than given d (WB)
        public bool IsCloserWB(PenguinGroup[] pGroups, int d1, PenguinGroup CpGroup)
        {
            for(int i = 0; i < pGroups.Length; i++)
            {
                int d2 = pGroups[i].TurnsTillArrival;
 
                if(IsOnBridge(pGroups[i]) || pGroups[i] == CpGroup)
                    d2 = PGDistanceWithBridge(pGroups[i]);
 
                if(d2 < d1)
                    return true;
            }
            return false;
        }
 
        // Builds bridges for existing exhibitions
        public void BuildBridges(PenguinGroup[] alliesP, IceBuilding ozar, PenguinGroup[] enemyP, Iceberg[] myI, Iceberg[] Neuts)
        {
            for (int i = 0; i < alliesP.Length; i++)
            {
                if (!alliesP[i].Destination.Equals(ozar))
                {
                    Iceberg destination = (Iceberg) alliesP[i].Destination;
                    Iceberg source = (Iceberg) alliesP[i].Source;
 
                    int d = 0;
 
                    if (AllPGToIce(source, enemyP).Length > 0)
                    {
                        d = ClosestGroup(source, enemyP).TurnsTillArrival;
 
                        if(IsOnBridge(ClosestGroup(source, enemyP)))
                            d = PGDistanceWithBridge(ClosestGroup(source, enemyP));
                    }
 
                    if (!IsOnBridge(alliesP[i]) && !IsWon(myI, Neuts, destination, alliesP, enemyP, destination.PenguinsPerTurn) && !source.AlreadyActed && source.PenguinAmount + source.PenguinsPerTurn * d - TotalExhibitions(source, enemyP) > source.BridgeCost && IsWonWithBridge(myI, Neuts, destination, alliesP, enemyP, destination.PenguinsPerTurn, alliesP[i]))
                        source.CreateBridge(destination);
                }
            }
        }
 
 
 
 
 
 
        //Core functions - the base of the DoTurn:
 
        public void Defense(Iceberg defended, Iceberg[] myIcebergs, PenguinGroup[] enemyP, PenguinGroup[] alliesP, Iceberg[] eI)
        {
 
 
            Iceberg[] defenders =  GetDefenders(myIcebergs, eI, alliesP, enemyP, defended);
 
 
            if(defenders.Length > 0)
            {
                Iceberg defender = defenders[0];
 
                int d1 = defender.GetTurnsTillArrival(defended);
                int p1 = defender.PenguinAmount; 
 
                if(BridgeExists(defender, defended))
                    d1 = IcebergDWithBridge(defender, defended);
 
                for(int i = 1; i < defenders.Length; i++)
                {
                    int d2 = defenders[i].GetTurnsTillArrival(defended);
                    int p2 = defenders[i].PenguinAmount;
 
                    if(BridgeExists(defenders[i], defended))
                        d2 = IcebergDWithBridge(defenders[i], defended);
 
                    if(d1 > d2 && !defenders[i].AlreadyActed)
                    {
                        defender = defenders[i];
                        d1 = d2;
                        p1 = p2;
                    }   
                    else
                    {
                        if(d1 == d2 && p2 > p1 && !defenders[i].AlreadyActed)
                        {
                            defender = defenders[i];
                            d1 = d2;
                            p1 = p2;
                        }
                    }
                }
 
                int needed = NewGetNeeded(defended, enemyP, alliesP, defender);
                defender.SendPenguins(defended,needed + 1);
            }
        }
 
        public Iceberg[] Attack(Iceberg[] myIcebergs, Iceberg myIceberg, Iceberg[] eI, Iceberg[] Neuts, PenguinGroup[] enemyP, PenguinGroup[] alliesP)
        {
 
            Iceberg target = GetTarget(myIcebergs, myIceberg, eI, Neuts, alliesP, enemyP);
 
            int d = myIceberg.GetTurnsTillArrival(target);
            //int f = myIceberg.CostFactor;
            int c = myIceberg.UpgradeCost;
 
            if(BridgeExists(myIceberg, target))
                d = IcebergDWithBridge(myIceberg, target);
 
            int needed = NewGetNeeded(target, enemyP, alliesP, myIceberg);
 
 
            if (needed >= 0 && (c - d >= needed || myIceberg.PenguinsPerTurn == 4) && CanSend(alliesP, enemyP, needed, myIceberg))
            {
                myIceberg.SendPenguins(target, needed + 1);
 
                if (IsNeutral(target, Neuts))
                {
                    Iceberg[] newNeuts = new Iceberg[Neuts.Length - 1];
 
                    int j = 0;
 
                    for (int i = 0; i < Neuts.Length; i++)
                    {
                        if (Neuts[i].UniqueId != target.UniqueId)
                        {
                            newNeuts[j] = Neuts[i];
                            j++;
                        }
                    }
                    return newNeuts;
                }
                else
                {
 
                    Iceberg[] newEI = new Iceberg[eI.Length - 1];
 
                    int j = 0;
 
                    for (int i = 0; i < eI.Length; i++)
                    {
                        if (eI[i].UniqueId != target.UniqueId)
                        {
                            newEI[j] = eI[i];
                            j++;
                        }
                    }
                    return newEI;
                }
            }
            else
            {
                if (myIceberg.CanUpgrade() && CanSend(alliesP, enemyP, myIceberg.UpgradeCost, myIceberg))
                    myIceberg.Upgrade();
 
                if(IsNeutral(target, Neuts))
                    return Neuts;
                return eI;
 
            }
 
        }
 
        public void DoStuff(Iceberg[] myI, Iceberg[] eI, Iceberg[] Neuts, PenguinGroup[] eP, PenguinGroup[] alliesP, IceBuilding ozar, Game game)
        {
            if (AllPenguins(myI) > AllPenguins(eI) +300) //Winning situation - spam their icebergs
            {
                for(int i = 0; i < myI.Length; i++)
                {
                    Iceberg target = GetTarget(myI, myI[i], eI, Neuts, alliesP, eP);
                    myI[i].SendPenguins(target, myI[i].PenguinAmount / 3);
                }
            }
 
            else
            {
                BuildBridges(alliesP, ozar, eP, myI, Neuts);
 
                //Run attack and defense over all icebergs
                int TreasureForce = - (int)(0.5 * ozar.PenguinAmount);
 
                for(int i = 0; i < myI.Length; i++)
                {
                    TreasureForce += TreasureFight(myI, ozar, myI[i], eI, eP, alliesP, TreasureForce);
 
                    int enemyPenguins = TotalExhibitions(myI[i], eP);
                    System.Console.WriteLine(CanSend(alliesP, eP, 0 , myI[i]) +" "+ myI[i]);
 
                    if(myI.Length > 1 && enemyPenguins > 0 && !CanSend(alliesP, eP, 0 , myI[i])) 
                        Defense(myI[i], myI, eP, alliesP, eI);
 
                    else
                    {
                        Iceberg target = GetTarget(myI, myI[i], eI, Neuts, alliesP, eP);
 
                        //SafeBridge(myI[i], target, alliesP, eP);
                        if (!myI[i].AlreadyActed)
                        {
                            if (IsNeutral(target, Neuts))
                            {
 
                                if (Neuts.Length > 0)
                                    Neuts = Attack(myI, myI[i], eI, Neuts, eP ,alliesP);
 
                                else
                                {
                                    if (myI[i].CanUpgrade() && CanSend(alliesP, eP, myI[i].UpgradeCost, myI[i]))
                                        myI[i].Upgrade();
                                }
                            }
 
                            else
                            {
                                if (eI.Length > 1)
                                    eI = Attack(myI, myI[i], eI, Neuts, eP ,alliesP);
 
                                else
                                {
                                    if (myI[i].CanUpgrade() && myI[i].PenguinAmount > myI[i].UpgradeCost + enemyPenguins)
                                        myI[i].Upgrade();
                                }
                            }
                            if(myI[i].PenguinAmount > 500)
                                myI[i].SendPenguins(target, 250);
                        }
                    }
                }
            }
        }
 
 
 
 
 
 
 
 
        public void DoTurn (Game game) 
        {
           Iceberg[] myI = game.GetMyIcebergs();
           Iceberg[] Neuts = game.GetNeutralIcebergs();
           Iceberg[] eI = game.GetEnemyIcebergs();
 
           PenguinGroup[] myP = game.GetMyPenguinGroups();
           PenguinGroup[] eP = game.GetEnemyPenguinGroups();
 
           IceBuilding ozar = game.GetBonusIceberg();
 
           //Initial occupation
 
            /*if (game.Turn == 1)
               myI[0].SendPenguins (game.GetNeutralIcebergs()[0], 11);
 
            if (game.Turn == 2)
                myI[0].SendPenguins (game.GetNeutralIcebergs()[1], 11);
 
            if (game.Turn>5)*/
 
            if (game.Turn > 150 && game.Turn % 20 == 0 && myI.Length == 1 && eI.Length == 1)
                myI[0].SendPenguins(Neuts[0], 50);
 
 
            DoStuff(myI, eI, Neuts, eP ,myP, ozar, game);
 
            /*
            PenguinGroup[] AlphaVisitors = SortCloseToFar(myI[0], eP, myP);
 
            System.Console.WriteLine("Alpha Visitors: ");
            for (int i = 0; i < AlphaVisitors.Length; i++){
                System.Console.WriteLine(AlphaVisitors[i]);
            }
            */
 
            System.Console.WriteLine(eI[0].Owner);
 
        }
    }
}
