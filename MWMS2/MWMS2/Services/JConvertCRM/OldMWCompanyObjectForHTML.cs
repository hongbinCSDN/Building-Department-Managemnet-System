
using System;
using System.Collections.Generic;

public class OldMWCompanyauthorizedSignatoryObject
{


    private string asName = "";
    private string asChineseName = "";

    private string typeOne = "";
    private string typeTwo = "";
    private string typeThree = "";

    private bool displayChi = false;




    public String[] getTypeLine(int index)
    {

        index = index - 1;
        String[] result = new String[] { "0", "" };

        List<string[]> typeLine = new List<string[]>();
        if (!typeOne.Equals(""))
        {
            typeLine.Add(new String[] { "1", typeOne });
        }
        if (!typeTwo.Equals(""))
        {
            typeLine.Add(new String[] { "2", typeTwo });
        }
        if (!typeThree.Equals(""))
        {
            typeLine.Add(new String[] { "3", typeThree });
        }

        try
        {
            if (typeLine.Count < index)
            {
                return result;
            }
            else
            {
                return typeLine[index];
            }
        }
        catch (Exception e)
        {
            return result;
        }
    }
    public string toString()
    {
        String result = "";
        result += "AS Name: " + getDisplayName() + Environment.NewLine;
        result += "AS Class I, II, III: " + getTypeOne() + Environment.NewLine;
        result += "AS Class II, III: " + getTypeTwo() + Environment.NewLine;
        result += "AS Class III: " + getTypeThree() + Environment.NewLine;
        return result;

    }


    public string getDisplayName()
    {
        if (displayChi)
        {
            String result = this.getAsChineseName();
            if (!result.Equals(""))
            {
                if (!this.getAsName().Equals(""))
                {
                    result += "<br>" + this.getAsName();
                }
            }
            else
            {
                result += " " + this.getAsName();
            }

            return result.ToUpper().Trim();
        }
        else
        {
            return this.getAsName().ToUpper().Trim();
        }
    }

    public int getLineNumber()
    {
        int result = 0;
        if (!(this.getTypeOne().Equals("")))
        {
            result += 1;
        }
        if (!(this.getTypeTwo().Equals("")))
        {
            result += 1;
        }
        if (!(this.getTypeThree().Equals("")))
        {
            result += 1;
        }
        if (result == 0)
        {
            result = 1;
        }

        return result;
    }




    public bool isDisplayChi()
    {
        return displayChi;
    }
    public void setDisplayChi(bool displayChi)
    {
        this.displayChi = displayChi;
    }
    public string getAsName()
    {
        return asName;
    }
    public void setAsName(String asName)
    {
        this.asName = asName;
    }
    public string getTypeOne()
    {
        return typeOne;
    }
    public void setTypeOne(String typeOne)
    {
        this.typeOne = typeOne;
    }
    public string getTypeTwo()
    {
        return typeTwo;
    }
    public void setTypeTwo(String typeTwo)
    {
        this.typeTwo = typeTwo;
    }
    public string getTypeThree()
    {
        return typeThree;
    }
    public void setTypeThree(String typeThree)
    {
        this.typeThree = typeThree;
    }

    public string getAsChineseName()
    {
        return asChineseName;
    }
    public void setAsChineseName(String asChineseName)
    {
        this.asChineseName = asChineseName;
    }

}

public class OldMWCompanyObjectForHTML {
	
	
	private string companyName ="";
	private string companyChineseName ="";
	
	private string typeOne ="";
	private string typeTwo ="";
	private string typeThree ="";
	private List<OldMWCompanyauthorizedSignatoryObject> authorizedSignatoryList =
		new List<OldMWCompanyauthorizedSignatoryObject>();

	private string registrationNumber ="";
	private string expiryDate ="";
	private string telephoneNumber ="";
	private string interestedFSS = "";
	private string interestedFSSChi = "";
	
	private bool displayChi = false;
	
	private string compRegionEng = "";
	private string compRegionChi = "";
	private string compRegion = "";
	private string emailAddress = "";
	private string bsFaxNumber = "";
	
	private string star = "";
	
	public string getCompRegion() {
		if(displayChi){
			return compRegionChi;
		}else{
			return compRegionEng;
		}
		
	}
	
	public string getDisplayName(){
		if(displayChi){
			String result = this.getCompanyChineseName();
			
			if(!result.Equals("")){
				if( !getCompanyName().Equals("")){
					result += "<br>" + this.getCompanyName();
				}
			}else{
				result += " " + this.getCompanyName();
			}
			
			return result.ToUpper().Trim();
		}else{
			return this.getCompanyName().ToUpper().Trim();
		}
		
	}
	public OldMWCompanyauthorizedSignatoryObject getASObject(int index){

        OldMWCompanyauthorizedSignatoryObject result; 
		
		if(index < authorizedSignatoryList.Count){
			result= authorizedSignatoryList[index];
		}else{
			result= new OldMWCompanyauthorizedSignatoryObject();	
		}
		result.setDisplayChi(isDisplayChi());
		return result;
		
	}
	
	public void addASObject(OldMWCompanyauthorizedSignatoryObject addObject){
		authorizedSignatoryList.Add(addObject);
	}
	
	
	public int getNumberOfLine(){
		int result =0;
		for(int i =0; i < authorizedSignatoryList.Count; i++ ){
			result = result +  (authorizedSignatoryList[i].getLineNumber() *2);
		}
		return result;
	}
	public int getNumberOfAS(){
		return authorizedSignatoryList.Count;
	}
	
	
	
	
	
	
	public string getCompanyName() {
		return companyName;
	}


	
	public void setCompanyName(String companyName) {
		this.companyName = companyName;
	}


	public string getTypeOne() {
		return typeOne;
	}


	public void setTypeOne(String typeOne) {
		this.typeOne = typeOne;
	}


	public string getTypeTwo() {
		return typeTwo;
	}


	public void setTypeTwo(String typeTwo) {
		this.typeTwo = typeTwo;
	}


	public string getTypeThree() {
		return typeThree;
	}


	public void setTypeThree(String typeThree) {
		this.typeThree = typeThree;
	}


	public string getRegistrationNumber() {
		return registrationNumber;
	}


	public void setRegistrationNumber(String registrationNumber) {
		this.registrationNumber = registrationNumber;
	}


	public string getExpiryDate() {
		return expiryDate;
	}


	public void setExpiryDate(String expiryDate) {
		this.expiryDate = expiryDate;
	}


	public string getTelephoneNumber() {
		return telephoneNumber;
	}


	public void setTelephoneNumber(String telephoneNumber) {
		this.telephoneNumber = telephoneNumber;
	}

	public string getInterestedFSS() {
		return interestedFSS;
	}
	
	public void setInterestedFSS(String interestedFSS){
		this.interestedFSS = interestedFSS;
	}
	
	public string getInterestedFSSChi() {
		return interestedFSSChi;
	}
	
	public void setInterestedFSSChi(String interestedFSSChi){
		this.interestedFSSChi = interestedFSSChi;
	}

	public List<OldMWCompanyauthorizedSignatoryObject> getAuthorizedSignatoryList() {
		return authorizedSignatoryList;
	}


	public void setAuthorizedSignatoryList(
            List<OldMWCompanyauthorizedSignatoryObject> authorizedSignatoryList) {
		this.authorizedSignatoryList = authorizedSignatoryList;
	}


	public string getCompanyChineseName() {
		return companyChineseName;
	}


	public void setCompanyChineseName(String companyChineseName) {
		this.companyChineseName = companyChineseName;
	}
	public bool isDisplayChi() {
		return displayChi;
	}
	public void setDisplayChi(bool displayChi) {
		this.displayChi = displayChi;
	}
	
	public string toString(){
		
		String result ="";
        result += "Company Name: " + this.getDisplayName() + Environment.NewLine;
		result += "Class I, II, III: "+ this.getTypeOne() + Environment.NewLine;
		result += "Class II, III: "+ this.getTypeTwo() + Environment.NewLine;
		result += "Class III: "+ this.getTypeThree() + Environment.NewLine;
		
		
		
		for(int i=0; i<  authorizedSignatoryList.Count ; i++){
			
			result += "AS "+i+ Environment.NewLine;
			result +=  authorizedSignatoryList[i].toString();
				
		}
		
		
		return result;
		
	}
	public string getCompRegionEng() {
		return compRegionEng;
	}
	public void setCompRegionEng(String compRegionEng) {
		this.compRegionEng = compRegionEng;
	}
	public string getCompRegionChi() {
		return compRegionChi;
	}
	public void setCompRegionChi(String compRegionChi) {
		this.compRegionChi = compRegionChi;
	}
	
	public void setCompRegion(String compRegion) {
		this.compRegion = compRegion;
	}
	public string getEmailAddress() {
		return emailAddress;
	}
	public void setEmailAddress(String emailAddress) {
		this.emailAddress = emailAddress;
	}
	public string getBsFaxNumber() {
		return bsFaxNumber;
	}
	public void setBsFaxNumber(String bsFaxNumber) {
		this.bsFaxNumber = bsFaxNumber;
	}

	public string getStar() {
		return star;
	}

	public void setStar(String star) {
		this.star = star;
	}
	
}
