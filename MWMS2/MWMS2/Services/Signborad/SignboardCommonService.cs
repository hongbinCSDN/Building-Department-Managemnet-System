using MWMS2.Entity;
using MWMS2.Services.Signborad.SignboardDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad
{
    public class SignboardCommonService
    {
        public void setRVDDataToSvAddress(B_SV_ADDRESS address)
        {
            try
            {
                string street = address.STREET;
                string streetNo = address.STREET_NO;
                string buildingName = address.BUILDINGNAME;
                bool isChinese = stringUtil.isChineseChar(street);

                B_RVD rvd = new B_RVD();

                if (address.UUID != null)
                {
                    rvd = getRVD(street, streetNo, buildingName,
                        stringUtil.isChineseChar(street));
                }
                if (rvd != null)
                {
                    address.RV_BLOCK_ID = rvd.RV_BLOCK_ID.ToString();
                    address.RV_STREET_CODE = rvd.RV_STREET_LOCATION_ID.ToString();
                }
                // The English format: Flat + Floor + Block No. + Building Name +
                // Street No + Street Name + District
                string display = address.FLAT + " "
                        + address.FLOOR + " "
                        + address.BLOCK + " "
                        + address.BUILDINGNAME + " "
                        + address.STREET_NO + " "
                        + address.STREET + " "
                        + address.DISTRICT;
                if (isChinese)
                {
                    // The Chinese format: District + Street Name + Street No +
                    // Building Name + Block No. + Floor + Flat
                    display = address.DISTRICT + " "
                            + address.STREET + " "
                            + address.STREET_NO + " "
                            + address.BUILDINGNAME + " "
                            + address.BLOCK + " "
                            + address.FLOOR + " "
                            + address.FLAT;
                }
                address.FULL_ADDRESS = display;

            }
            catch (Exception mnfe)
            {

            }
        }

        public B_RVD getRVD(string streeName, string streetNo, string buildingName, bool chinese)
        {
            List<B_RVD> resultList = null;
            B_RVD result = null;
            if (streetNo == null)
            {
                streetNo = "";
            }
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                if (chinese)
                {
                    resultList = db.B_RVD.Where
                        (o => o.STREET_NAME_CHN == streeName
                        && o.BLDG_NAME_CHN == buildingName
                        && (o.BK_BLDG_NO_NUMERIC == Int32.Parse(streetNo) || o.BK_BLDG_NO_ALPHA == streetNo || o.BK_BLDG_NO_EXT == streetNo)).ToList();
                }
                else
                {
                    resultList = db.B_RVD.Where
                        (o => o.STREET_NAME_ENG.ToUpper() == streeName.ToUpper()
                        && (o.BK_BLDG_NO_NUMERIC == Int32.Parse(streetNo) || o.BK_BLDG_NO_ALPHA == streetNo || o.BK_BLDG_NO_EXT == streetNo)).ToList();
                }
            }
            if (resultList != null)
            {
                result = resultList[0];
            }
            return result;
        }
        public B_SV_RV_ADDRESS getSvRVSignboardAddress(B_SV_ADDRESS address)
        {
            RvBlockDAOService rvBlockDAOService = new RvBlockDAOService();
            B_SV_RV_ADDRESS svRvAddress = new B_SV_RV_ADDRESS();

            RvStreetLocationDAOService rvStreetLocationDAOService = new RvStreetLocationDAOService();
            RvStreetNameDAOService rvStreetNameDAOService = new RvStreetNameDAOService();
            RvLocationNameDAOService rvLocationNameDAOService = new RvLocationNameDAOService();
            RvBuildingDAOService rvBuildingDAOService = new RvBuildingDAOService();
            RvDevelopmentNameDAOService rvDevelopmentNameDAOService = new RvDevelopmentNameDAOService();
            try
            {
                using (EntitiesSignboard db = new EntitiesSignboard())
                {
                    List<B_SV_RV_ADDRESS> resultList = db.B_SV_RV_ADDRESS.Where(o => o.UUID == address.UUID).ToList();
                    if (resultList != null)
                    {
                        svRvAddress = resultList[0];
                    }
                    else
                    {
                        svRvAddress = new B_SV_RV_ADDRESS();
                        svRvAddress.B_SV_ADDRESS = address;
                    }
                    svRvAddress.DISPLAY_BLOCK_ID = address.RV_BLOCK_ID;
                    svRvAddress.DISPLAY_STREET_CODE = address.RV_STREET_CODE;

                    svRvAddress.DISPLAY_STREET = address.STREET;
                    svRvAddress.DISPLAY_STREET_NO = address.STREET_NO;
                    svRvAddress.DISPLAY_BUILDINGNAME = address.BUILDINGNAME;

                    svRvAddress.DISPLAY_FLOOR = address.FLOOR;
                    svRvAddress.DISPLAY_FLAT = address.FLAT;
                    svRvAddress.DISPLAY_DISTRICT = address.DISTRICT;
                    svRvAddress.DISPLAY_REGION = address.REGION;

                    svRvAddress.ENGLISH_DISPLAY = address.FULL_ADDRESS;
                    svRvAddress.CHINESE_DISPLAY = address.FULL_ADDRESS;

                    svRvAddress.ENGLISH_RRM_ADDRESS = address.FULL_ADDRESS;
                    svRvAddress.ENGLISH_RRM_BUILDING = address.STREET
                        + " "
                        + address.STREET_NO
                        + " "
                        + address.BUILDINGNAME;
                    svRvAddress.CHINESE_RRM_ADDRESS = address.FULL_ADDRESS;

                    svRvAddress.CHINESE_RRM_BUILDING = address
                            .STREET
                            + " "
                            + address.STREET_NO
                            + " "
                            + address.BUILDINGNAME;

                    svRvAddress.BLOCK = address.BLOCK;
                    svRvAddress.BCIS_BLOCK_ID = address.BCIS_BLOCK_ID;
                    svRvAddress.FILE_REFERENCE_NO = address.FILE_REFERENCE_NO;
                    svRvAddress.CHINESE_RRM_BUILDING = address.STREET
                            + " "
                            + address.STREET_NO
                            + " "
                            + address.BUILDINGNAME;

                    if (address.RV_BLOCK_ID != null
                            && address.RV_STREET_CODE != null)
                    {
                        long blockId = Int32.Parse(address.RV_BLOCK_ID);
                        long streetId = Int32.Parse(address.RV_STREET_CODE);
                        B_RV_BLOCK rvBlock = rvBlockDAOService.findByBlockIdAndStreetCode(blockId, streetId);

                        // Building & Block.
                        if (rvBlock != null)
                        {
                            svRvAddress.BLOCK_ID_ALPHA = rvBlock.BK_BLK_ID_ALPHA;
                            svRvAddress.BLOCK_ID_ALPHA_PRE_INDICATOR = rvBlock
                                    .BK_BLK_ID_ALPHA_PREC_IND;
                            svRvAddress.ENGLISH_BLOCK_DESCRIPTION = rvBlock
                                    .BK_BLK_DESC;
                            svRvAddress.CHINESE_BLOCK_DESCRIPTION = rvBlock
                                    .BK_BLK_DESC_CHN;
                            svRvAddress.BLOCK_DESC_PRECEDE_INDICATOR = rvBlock
                                    .BK_DESC_PREC_IND;
                            svRvAddress.ENGLISH_BUILDING_NAME_LINE_1 = rvBlock.BK_BLDG_NAME_ENG_LINE_1;
                            svRvAddress.ENGLISH_BUILDING_NAME_LINE_2 = rvBlock.BK_BLDG_NAME_ENG_LINE_2;
                            svRvAddress.ENGLISH_BUILDING_NAME_LINE_3 = rvBlock.BK_BLDG_NAME_ENG_LINE_3;
                            svRvAddress.CHINESE_BUILDING_NAME_LINE_1 = rvBlock.BK_BLDG_NAME_CHN_LINE_1;
                            svRvAddress.CHINESE_BUILDING_NAME_LINE_2 = rvBlock.BK_BLDG_NAME_CHN_LINE_2;
                            svRvAddress.CHINESE_BUILDING_NAME_LINE_3 = rvBlock.BK_BLDG_NAME_CHN_LINE_3;
                            svRvAddress.ENGLISH_BLOCK_ADDRESS = rvBlock.BK_FULL_ADDR_ENG;
                            svRvAddress.ENGLISH_BLOCK_ADDRESS_2 = rvBlock.BK_FULL_ADDR2_ENG;
                            svRvAddress.CHINESE_BLOCK_ADDRESS = rvBlock.BK_FULL_ADDR_CHN;

                            // Street Location.
                            if (rvBlock.BK_SL_TABLE_ID != null)
                            {
                                //Rv_Street_LocationDAO rvStreetLocationDAO = new RvStreetLocationDAO=
                                //        session);
                                B_RV_STREET_LOCATION rvStreetLocation = rvStreetLocationDAOService.findBySlTableId(rvBlock.BK_SL_TABLE_ID);

                                if (rvStreetLocation != null)
                                {

                                    B_RV_STREET_NAME rvStreetName = null;
                                    B_RV_LOCATION_NAME rvLocationName1 = null;
                                    B_RV_LOCATION_NAME rvLocationName2 = null;
                                    B_RV_LOCATION_NAME rvLocationName3 = null;

                                    //RvStreetNameDAO rvStreetNameDAO = new RvStreetNameDAO=
                                    //        session);
                                    //RvLocationNameDAO rvLocationNameDAO = new RvLocationNameDAO=
                                    //        session);

                                    if (rvStreetLocation.SL_SM_TABLE_ID != null)
                                    {
                                        rvStreetName = rvStreetNameDAOService.findBySmTableId(rvStreetLocation.SL_SM_TABLE_ID);
                                        if (rvStreetName != null)
                                        {
                                            svRvAddress.STREE_NAME = rvStreetName
                                                    .SM_NAME_CHN;
                                            svRvAddress
                                                    .ENGLISH_STREET_NAME = rvStreetName
                                                            .SM_NAME_ENG;
                                            svRvAddress
                                                    .ENGLISH_STREET_TYPE = rvStreetName
                                                            .SM_TYPE_ENG;
                                            svRvAddress
                                                    .ENGLISH_STREET_DIRECTION = rvStreetName
                                                            .SM_DIRECTION_ENG;
                                            svRvAddress
                                                    .ENGLISH_ST_TYPE_PRE_INDICATOR = rvStreetName
                                                            .SM_TYPE_ENG_PREC_IND;
                                            svRvAddress
                                                    .CHINESE_STREET_NAME = rvStreetName
                                                            .SM_NAME_CHN;
                                            svRvAddress
                                                    .CHINESE_STREET_TYPE = rvStreetName
                                                            .SM_TYPE_CHN;
                                            svRvAddress
                                                    .CHINESE_STREET_DIRECTION = rvStreetName
                                                            .SM_DIRECTION_CHN;
                                            svRvAddress
                                                    .CHINESE_ST_TYPE_PRE_INDICATOR = rvStreetName
                                                            .SM_TYPE_CHN_PREC_IND;
                                        }
                                    }

                                    if (rvStreetLocation.SL_LC_TABLE_ID1 != null)
                                    {

                                        rvLocationName1 = rvLocationNameDAOService.findByLcTableId(rvStreetLocation.SL_LC_TABLE_ID1);

                                        if (rvLocationName1 != null)
                                        {
                                            svRvAddress
                                                    .ENGLISH_ST_LOCATION_NAME_1 = rvLocationName1
                                                            .LC_NAME_ENG;
                                            svRvAddress
                                                    .CHINESE_ST_LOCATION_NAME_1 = rvLocationName1
                                                            .LC_NAME_CHN;
                                        }
                                    }

                                    if (rvStreetLocation.SL_LC_TABLE_ID2 != null)
                                    {
                                        rvLocationName2 = rvLocationNameDAOService.findByLcTableId(rvStreetLocation.SL_LC_TABLE_ID2);

                                        if (rvLocationName2 != null)
                                        {
                                            svRvAddress
                                                    .ENGLISH_ST_LOCATION_NAME_2 = rvLocationName2
                                                            .LC_NAME_ENG;
                                            svRvAddress
                                                    .CHINESE_ST_LOCATION_NAME_2 = rvLocationName2
                                                            .LC_NAME_CHN;
                                        }
                                    }

                                    if (rvStreetLocation.SL_LC_TABLE_ID3 != null)
                                    {
                                        rvLocationName3 = rvLocationNameDAOService.findByLcTableId(rvStreetLocation.SL_LC_TABLE_ID3);

                                        if (rvLocationName3 != null)
                                        {
                                            svRvAddress
                                                    .ENGLISH_ST_LOCATION_NAME_3 = rvLocationName3
                                                            .LC_NAME_ENG;
                                            svRvAddress
                                                    .CHINESE_ST_LOCATION_NAME_3 = rvLocationName3
                                                            .LC_NAME_CHN;
                                        }
                                    }
                                }
                            }

                            // Development Name.
                            if (rvBlock.BK_BG_TABLE_ID != null)
                            {
                                //RvBuildingDAO rvBuildingDAO = new RvBuildingDAO=session);
                                B_RV_BUILDING rvBuilding = rvBuildingDAOService.findByBgTableId(rvBlock.BK_BG_TABLE_ID);

                                if (rvBuilding != null && rvBuilding.BG_DV_TABLE_ID != null)
                                {
                                    long developmentNameTableId = Convert.ToInt64(rvBuilding.BG_DV_TABLE_ID);

                                    //RvDevelopmentNameDAO rvDevelopmentNameDAO = new RvDevelopmentNameDAO=session;
                                    B_RV_DEVELOPMENT_NAME rvDevelopmentName = rvDevelopmentNameDAOService.findByDvTableId(Convert.ToInt32(developmentNameTableId));

                                    if (rvDevelopmentName != null)
                                    {
                                        svRvAddress
                                                .ENGLISH_DEVELOPMENT_NAME = rvDevelopmentName
                                                        .DV_NAME_ENG;
                                        svRvAddress
                                                .CHINESE_DEVELOPMENT_NAME = rvDevelopmentName
                                                        .DV_NAME_CHN;
                                    }
                                }
                            }
                        }
                    }

                    svRvAddress.BUILDING_NAME = address.BUILDINGNAME;
                    return svRvAddress;
                }
            }
            catch
            {


            }
            return svRvAddress;
        }

    }
}