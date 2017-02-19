using IMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Data
{
    public class GZShenQingRepository : RepositoryBase<GZShenQing>
    {
        SBXXRespository SBXXRespository = new SBXXRespository();

        public IEnumerable<SBXX> GetShortNameList()
        {
            try
            {
                IEnumerable<SBXX> ShortNameList = SBXXRespository.GetList();
                return ShortNameList;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool AddNewFailure(GZShenQing GZTiJiao)
        {
            try
            {
                bool s = this.Insert(GZTiJiao);
                return s;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<GZShenQing> GetAllApplicationsByName(string name)
        {
            try
            {
                IEnumerable<GZShenQing> AllApplicationsList =this.GetList(new { bgrxm = name }, null,false);
                return AllApplicationsList;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
