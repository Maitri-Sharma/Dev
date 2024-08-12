using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Puma.Shared
{
    [Serializable()]
    public class ReolCollection : System.Collections.Generic.List<Reol>
    {
        public ReolCollection() : base()
        {
        }

        public ReolCollection(ReolCollection reolColl) : base()
        {
            this.AddRange(reolColl);
        }

        // Adds all the given reoler that are not already present in the collection.
        public void AddRangeUnique(ReolCollection reoler)
        {
            XANDCollection(reoler);
        }

        public ReolCollection CreateCopy()
        {
            ReolCollection result = new ReolCollection();
            foreach (Reol r in this)
                result.Add(r);
            return result;
        }

        public List<long> GetReolIDs()
        {
            List<long> result = new List<long>();
            foreach (Reol r in this)
                result.Add(r.ReolId);
            return result;
        }

        public string GetReolIDsInClause(string idfield)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.Append(idfield);
            sql.Append(" IN (");
            int i = 0;
            foreach (long id in this.GetReolIDs())
            {
                i += 1;
                if ((i > 100))
                {
                    sql.Remove(sql.Length - 1, 1);
                    sql.Append(") OR ");
                    sql.Append(idfield);
                    sql.Append(" IN (");
                    i = 0;
                }
                sql.Append("'");
                sql.Append(id.ToString().Trim());
                sql.Append("',");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");
            return sql.ToString();
        }
        public string GetReolIDsInClause(string idfield, IList<long> RemoveIDs, ref IList<long> removedIDs)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.Append(idfield);
            sql.Append(" IN (");
            int i = 0;
            bool buildedSQL = false;
            if (removedIDs == null)
                removedIDs = new List<long>();
            foreach (long id in this.GetReolIDs())
            {
                if (!RemoveIDs.Contains(id))
                {
                    buildedSQL = true;
                    i += 1;
                    if ((i > 100))
                    {
                        sql.Remove(sql.Length - 1, 1);
                        sql.Append(") OR ");
                        sql.Append(idfield);
                        sql.Append(" IN (");
                        i = 0;
                    }
                    sql.Append("'");
                    sql.Append(id.ToString().Trim());
                    sql.Append("',");
                }
                else
                    removedIDs.Add(id);
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");
            if (!buildedSQL)
                return "";
            return sql.ToString();
        }

        public void XORWithCollection(ReolCollection reoler)
        {
            foreach (Reol r in reoler)
            {
                if (this.ContainsId(r.ReolId))
                    this.RemoveId(r.ReolId);
                else
                    this.Add(r);
            }
        }

        public void XANDCollection(ReolCollection reoler)
        {
            foreach (Reol r in reoler)
            {
                if (!this.ContainsId(r.ReolId))
                    this.Add(r);
            }
        }

        public AntallInformation FindAntall()
        {
            AntallInformation sum = new AntallInformation();
            foreach (Reol r in this)
                if (r != null)
                {
                    sum.Accumulate(r.Antall);
                }
            return sum;
        }

        public AntallInformation FindAntallForPriceZone(int priceZone)
        {
            AntallInformation sum = new AntallInformation();
            foreach (Reol r in this)
            {
                if (r.PrisSone == priceZone)
                    sum.Accumulate(r.Antall);
            }
            return sum;
        }

        public ReolTree GroupByFylkeKommune()
        {
            ReolTree result = new ReolTree();
            foreach (Reol r in this)
                result.InsertByFylkeKommune(r);
            return result;
        }

        public ReolTree GroupByKommune()
        {
            ReolTree result = new ReolTree();
            foreach (Reol r in this)
                result.InsertByKommune(r);
            return result;
        }

        public ReolTree GroupByFylkeKommuneTeam()
        {
            // SortByFylkeKommuneTeam()
            SortByFylkeKommuneTeamNameReolNoReolName();
            ReolTree result = new ReolTree();
            foreach (Reol r in this)
                result.InsertByFylkeKommuneTeam(r);
            return result;
        }

        public ReolTree GroupByPostalZone()
        {
            SortByPostalZoneReolNo();
            ReolTree result = new ReolTree();
            foreach (Reol r in this)
                result.InsertByPostalZone(r);
            return result;
        }

        public ReolTree GroupByTeamReol()
        {
            SortByTeamReol();
            ReolTree result = new ReolTree();
            foreach (Reol r in this)
                result.InsertByTeam(r);
            return result;
        }

        public void SortByTeamReol()
        {
            this.Sort(new ReolComparerByTeamReol());
        }

        public void SortByPostalZoneReol()
        {
            this.Sort(new PostalzoneComparerByPostalZoneReol());
        }

        public void SortByPostalZoneReolNo()
        {
            this.Sort(new PostalzoneComparerByPostalZoneReolNo());
        }


        public void SortByFylkeKommuneTeam()
        {
            this.Sort(new ReolComparerByFylkeKommuneTeam());
        }

        public void SortByReolName()
        {
            this.Sort(new ReolComparerByReolName());
        }

        public void SortByReolNumber()
        {
            this.Sort(new ReolComparerByReolNumber());
        }

        public void SortByTeamNameReolNoReolName()
        {
            this.Sort(new ReolComparerByTeamNameReolNoReolName());
        }

        public void SortByFylkeKommuneTeamNameReolNoReolName()
        {
            this.Sort(new ReolComparerByFylkeKommuneTeamNameReolNoReolName());
        }

        public bool ContainsId(long reolId)
        {
            foreach (Reol r in this)
            {
                if (r.ReolId == reolId)
                    return true;
            }
            return false;
        }

        public Reol GetReol(long reolId)
        {
            foreach (Reol r in this)
            {
                if (r.ReolId == reolId)
                    return r;
            }
            return null/* TODO Change to default(_) if this is not a reference type */;
        }

        public bool ReolExist(long reolId)
        {
            foreach (Reol r in this)
            {
                if (r.ReolId == reolId)
                    return true;
            }
            return false;
        }

        public ReolCollection GetReolsInFylke(string fylkeId)
        {
            ReolCollection result = new ReolCollection();
            foreach (Reol r in this)
            {
                if (r.FylkeId == fylkeId)
                    result.Add(r);
            }
            return result;
        }

        public ReolCollection GetReolsInKommune(string KommuneId)
        {
            ReolCollection result = new ReolCollection();
            foreach (Reol r in this)
            {
                if (r.KommuneId == KommuneId)
                    result.Add(r);
            }
            return result;
        }

        public ReolCollection GetReolsInTeam(string TeamName)
        {
            ReolCollection result = new ReolCollection();
            foreach (Reol r in this)
            {
                if (r.TeamName == TeamName)
                    result.Add(r);
            }
            return result;
        }

        public ReolCollection GetReolsByTeamNr(string teamNr)
        {
            ReolCollection result = new ReolCollection();
            foreach (Reol r in this)
            {
                if (r.TeamNumber == teamNr)
                    result.Add(r);
            }
            return result;
        }

        public ReolCollection GetReolsByKommuneIdAndTeamNr(string kommuneId, string teamNr)
        {
            ReolCollection result = new ReolCollection();
            foreach (Reol r in this)
            {
                if (r.KommuneId == kommuneId && r.TeamNumber == teamNr)
                    result.Add(r);
            }
            return result;
        }

        public ReolCollection GetReolsInPostnummer(string Postnr)
        {
            ReolCollection result = new ReolCollection();
            foreach (Reol r in this)
            {
                if (r.PostalZone == Postnr)
                    result.Add(r);
            }
            return result;
        }

        /// <summary>
        ///     ''' Returns a collection of reols which have names which starts with the input text
        ///     ''' </summary>
        ///     ''' <param name="searchNameText"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks>Is not case sensitive</remarks>
        public ReolCollection GetReolsByNameSearch(string searchNameText)
        {
            ReolCollection result = new ReolCollection();

            if (searchNameText != null)
            {
                foreach (Reol r in this)
                {
                    if (r.Name != null)
                    {
                        if (r.Name.ToLower().Contains(searchNameText.ToLower()))
                            result.Add(r);
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     ''' Returns a collection of reols which have names which starts with the input text
        ///     ''' </summary>
        ///     ''' <param name="searchNameText"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks>Is not case sensitive</remarks>
        public ReolCollection GetReolsPostboksByNameSearch(string searchNameText, string kommuneName)
        {
            ReolCollection result = new ReolCollection();

            if (searchNameText != null)
            {
                if (kommuneName != "")
                {
                    foreach (Reol r in this)
                    {
                        if (r.Name.ToLower().Contains(searchNameText.ToLower()) & r.RuteType.ToLower() == "boks" & r.Kommune.ToLower().Contains(kommuneName.ToLower()))
                            // If r.Name.ToLower().Contains(searchNameText.ToLower()) Then 
                            result.Add(r);
                    }
                }
                else
                    foreach (Reol r in this)
                    {
                        if (r.Name.ToLower().Contains(searchNameText.ToLower()) == true & r.RuteType.ToLower() == "boks")
                            // If r.Name.ToLower().Contains(searchNameText.ToLower()) Then 
                            result.Add(r);
                    }
            }

            return result;
        }

        public void RemoveId(long reolId)
        {
            foreach (Reol r in new ArrayList(this))
            {
                if (r.ReolId == reolId)
                    Remove(r);
            }
        }

        // Adds the given reol if not already present in the collection.
        public void AddUnique(Reol reol)
        {
            if (!this.ContainsId(reol.ReolId))
                this.Add(reol);
        }

        // returns all reoler existing in both collections as a ReolCollection
        public ReolCollection GetDoubleReolcoverage(ReolCollection reoler)
        {
            ReolCollection reolColl = new ReolCollection();
            foreach (Reol rMe in this)
            {
                foreach (Reol r in reoler)
                {
                    if (rMe.ReolId == r.ReolId)
                        reolColl.AddUnique(rMe);
                }
            }
            return reolColl;
        }
    }

    public class ReolComparerByTeamReol : Comparer<Reol>
    {
        public override int Compare(Reol x, Reol y)
        {
            int result = x.TeamName.CompareTo(y.TeamName);
            if (result == 0)
                result = x.ReolNumber.CompareTo(y.ReolNumber);
            return result;
        }
    }

    public class ReolComparerByTeamNameReolNoReolName : Comparer<Reol>
    {
        public override int Compare(Reol x, Reol y)
        {
            int result = x.TeamName.CompareTo(y.TeamName);
            if (result == 0)
                result = x.ReolNumber.CompareTo(y.ReolNumber);
            if (result == 0)
                result = x.Name.CompareTo(y.Name);
            return result;
        }
    }

    public class ReolComparerByFylkeKommuneTeamNameReolNoReolName : Comparer<Reol>
    {
        public override int Compare(Reol x, Reol y)
        {
            int result = x.Fylke.CompareTo(y.Fylke);
            if (result == 0)
                result = x.Kommune.CompareTo(y.Kommune);
            if (result == 0)
                result = x.TeamName.CompareTo(y.TeamName);
            if (result == 0)
                result = x.ReolNumber.CompareTo(y.ReolNumber);
            if (result == 0)
                result = x.Name.CompareTo(y.Name);
            return result;
        }
    }

    public class ReolComparerByFylkeKommuneTeam : Comparer<Reol>
    {
        public override int Compare(Reol x, Reol y)
        {
            int result = x.Fylke.CompareTo(y.Fylke);
            if (result == 0)
                result = x.Kommune.CompareTo(y.Kommune);
            if (result == 0)
                result = x.TeamName.CompareTo(y.TeamName);
            if (result == 0)
                result = x.Name.CompareTo(y.Name);
            if (result == 0)
                result = x.ReolId.CompareTo(y.ReolId);
            return result;
        }
    }

    public class ReolComparerByReolName : Comparer<Reol>
    {
        public override int Compare(Reol x, Reol y)
        {
            if (x != null & y == null)
                return -1;
            if (x == null & y == null)
                return 0;
            if (x == null & y != null)
                return 1;

            return x.Name.CompareTo(y.Name);
        }
    }

    public class ReolComparerByReolNumber : Comparer<Reol>
    {
        public override int Compare(Reol x, Reol y)
        {
            if (x != null & y == null)
                return -1;
            if (x == null & y == null)
                return 0;
            if (x == null & y != null)
                return 1;

            int result = x.ReolNumber.CompareTo(y.ReolNumber);
            if (result == 0)
                result = x.Name.CompareTo(y.Name);
            return result;
        }
    }
}
