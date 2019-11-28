
using System;

public class OldDateUtil { 

	public static long DAY = 24*60*60*1000;
	public static long MINUTE = 60*1000;
	
	public static int MILLISECS_PER_DAY = 24*60*60*1000;
	public static DateTime D20040831 = new DateTime(1093881600000L);
	
	public static string DATE_DISPLAY_FORMAT ="dd/MM/yyyy";
	
	public static string DATE_SQL_FORMAT ="yyyyMMdd";
	
	public static string DATETIME_DISPLAY_FORMAT ="dd/MM/yyyy hh:mm aaa";
	
	public static string EXPORT_DATE_FORMAT ="dd/MM/yyyy HH:mm:ss";
	public static string FULL_ENGLISH_DATE_WITH_DAY_OF_WEEK = "EEEEE, d MMMMM yyyy";
	public static string FULL_ENGLISH_DATE_WITHOUT_DAY = "MMMMM yyyy";
	public static string TIME_DISPLAY_FORMAT =" hh:mm a";
	public static string TIME_24_DISPLAY_FORMAT ="HH:mm";


    /*
    public static string getDateDisplayFormat(string dateinput){
        return getDateDisplayFormat(getDisplayDateToDBDate(dateinput));
    }*/

    public static string getDateDisplayFormat(object o)
    {
        if (o == null)
        {
            return "";
        }
        else if (o.GetType() == typeof(DateTime))
        {
            return getDateDisplayFormat((DateTime)o);
        }
        else
        {
            return o.ToString();
        }
    }


    public static string getDateDisplayFormat(DateTime input)
    {
        if (input != null)
        {
            return input.ToString(DATE_DISPLAY_FORMAT);
            //Format formatter;
            //formatter = new SimpleDateFormat(DATE_DISPLAY_FORMAT);
            //return formatter.format(input);
        }
        else
        {
            return "";
        }
    }



    /*
    public static string getDateDisplayFormat(Timestamp input) {
        if (input!= null){
            Format formatter;
            formatter = new SimpleDateFormat(DATE_DISPLAY_FORMAT);
            return formatter.format(input);
        }else{
            return "";
        }
   }

    public static string getDateDisplayFormat(Date input) {
        if (input!= null){
            Format formatter;
            formatter = new SimpleDateFormat(DATE_DISPLAY_FORMAT);
            return formatter.format(input);
        }else{
            return "";
        }
   }

    public static string getDateTimeDisplayFormat(Date input) {
        if (input!= null){
            Format formatter;
            formatter = new SimpleDateFormat(DATETIME_DISPLAY_FORMAT, Locale.ENGLISH);
            return formatter.format(input);
        }else{
            return "";
        }
   }

    */

    public static string getExportDateDisplay(DateTime? input) {
		 if (input!= null){
            return input.Value.ToString(EXPORT_DATE_FORMAT);
			 //Format formatter;
			 //formatter = new SimpleDateFormat(EXPORT_DATE_FORMAT, Locale.ENGLISH);
			 //return formatter.format(input);
		 }else{
			 return "";
		 }
    }
    /*
	 
	public static string getTimeDisplayFormat(Date input) {
		 if (input!= null){
			 Format formatter;
			 formatter = new SimpleDateFormat(TIME_DISPLAY_FORMAT, Locale.ENGLISH);
			 return formatter.format(input);
		 }else{
			 return "";
		 }
    }
	
	public static string get24TimeDisplayFormat(Date input) {
		 if (input!= null){
			 Format formatter;
			 formatter = new SimpleDateFormat(TIME_24_DISPLAY_FORMAT, Locale.ENGLISH);
			 return formatter.format(input);
		 }else{
			 return "";
		 }
   }
	
	public static DateTime getDBDate(Date anyformatDate){
		return  getDisplayDateToDBDate(getDateDisplayFormat(anyformatDate));
	}
	 
	public static DateTime getDisplayDateToDBDate(s                                                                                                                                                                                                                                                                                                                                                     tring displayDate) {

		if(displayDate == null){
			return null;
		}
		
		displayDate = displayDate.trim().replaceAll("[.]", "/").replaceAll("[-]", "/");		
		
		if (!displayDate.equals("")){
			DateFormat dateformat = new SimpleDateFormat(DATE_DISPLAY_FORMAT);
			 DateTime result = null;
			 try{ 
				 result = dateformat.parse(displayDate);
			 }catch(Exception e){
				 return result;
			 }
			 return result;
		 }else{
			 return null;
		 }
     }
	
	public static string getSQLDisplay(String displayDate, string sDATE_SQL_FORMAT) {
		Date date = getDisplayDateToDBDate(displayDate);
		 if (date != null){
			 Format formatter;
			 formatter = new SimpleDateFormat(sDATE_SQL_FORMAT);
			 return formatter.format(date);
		 }else{
			 return "01/01/2001";
		 }
    }
	
	public static int getDayDiff(Date start, DateTime end){
		if((start == null) || (end == null)){
			return 0;
		}
		long starttime = start.getTime();
		long endtime = end.getTime();
		int result = (int) ( ( ( starttime - endtime) )/MILLISECS_PER_DAY);
		return  result;
	}
	
	
	public static DateTime getInc(Date start, int numberOfDays){
		Calendar cal = Calendar.getInstance();
		cal.setTime(start);
		cal.add(Calendar.DATE, numberOfDays);
		return cal.getTime();
	}
	
	public static boolean isWeekDay(Date date){
		
		Calendar cal = Calendar.getInstance();
		cal.setTime(date);
		int i = cal.get(Calendar.DAY_OF_WEEK);
		
		if((i == Calendar.SUNDAY) || (i == Calendar.SATURDAY)){
			return false;
		}else{
			return true;
		}
	}

	
	
	public static int getCurrentYear(){
		Calendar calendar = Calendar.getInstance();
		return calendar.get(java.util.Calendar.YEAR);
	}
	
	
	public static string getLongCurrentDate(){
		Format formatter = new SimpleDateFormat(DATE_DISPLAY_FORMAT);
		return formatter.format(new Date());
	}
	
	
	public static string getChineseFormatDateWithTime(Date date){
		
		if(date == null){
			return "";
		}
		
		Calendar calendar = Calendar.getInstance();
		calendar.setTime(date);
		int year = calendar.get(java.util.Calendar.YEAR);
		int month = calendar.get(java.util.Calendar.MONTH)+1;
		int dates = calendar.get(java.util.Calendar.DATE);
		int hr = calendar.get(java.util.Calendar.HOUR);
		String hrStr = "";
		if(hr==0){
			hr=12;
		}
		if(hr<10){
			hrStr = "0"+hr;
		}else{
			hrStr = String.valueOf(hr);
		}
		int am_pm = calendar.get(java.util.Calendar.AM_PM);
		String am_pmStr = "";
		if (am_pm==0) 
	       {  am_pmStr="上午";
	       }
	       else
	       { am_pmStr="下午";
	       }

		int min =  calendar.get(java.util.Calendar.MINUTE);
		String minStr = "";
		if(min<10){
			minStr = "0"+min;
		}else{
			minStr = String.valueOf(min);
		}
		
		return  year + "年" + month+"月" +dates+"日 "+am_pmStr+hrStr+"時"+minStr+"分";
	}
	
	public static string getChineseFormatDateWithTimeForLetterExport(Date date){
		
		if(date == null){
			return "";
		}
		
		Calendar calendar = Calendar.getInstance();
		calendar.setTime(date);
		int year = calendar.get(java.util.Calendar.YEAR);
		int month = calendar.get(java.util.Calendar.MONTH)+1;
		int dates = calendar.get(java.util.Calendar.DATE);
		int hr = calendar.get(java.util.Calendar.HOUR);
		String hrStr = "";
		if(hr==0){
			hr=12;
		}
		hrStr = String.valueOf(hr);
		
		int am_pm = calendar.get(java.util.Calendar.AM_PM);
		String am_pmStr = "";
		if (am_pm==0) 
	       {  am_pmStr="上午";
	       }
	       else
	       { am_pmStr="下午";
	       }

		int min =  calendar.get(java.util.Calendar.MINUTE);
		String minStr = "";
		if(min<10){
			minStr = "0"+String.valueOf(min);
		}else{			
			minStr = String.valueOf(min);
		}
		
		
		return  year + "年" + month+"月" +dates+"日"+am_pmStr+hrStr+"時"+minStr+"分";
	}
	
	public static string getEnglsihFormatDateWithTimeForLetterExport(Date date) {
		 if(date == null){
				return "";
			}
			Calendar calendar = Calendar.getInstance();
			calendar.setTime(date);
			int year = calendar.get(java.util.Calendar.YEAR);
			int month = calendar.get(java.util.Calendar.MONTH)+1;
			int dates = calendar.get(java.util.Calendar.DATE);
			int hr = calendar.get(java.util.Calendar.HOUR);
			String hrStr = "";
			if(hr==0){
				hr=12;
			}
			hrStr = String.valueOf(hr);
			
			int am_pm = calendar.get(java.util.Calendar.AM_PM);
			String am_pmStr = "";
			if (am_pm==0) 
		       {  am_pmStr="a.m.";
		       }
		       else
		       { am_pmStr="p.m.";
		       }

			int min =  calendar.get(java.util.Calendar.MINUTE);
			String minStr = "";
			if(min<10){
				minStr = "0"+String.valueOf(min);
			}else{			
				minStr = String.valueOf(min);
			}
			
			return  dates +" "+getFullMonth(month) + " "+ year +" "+hrStr+":"+minStr+am_pmStr;
			
			
   }
	public static string getDateDisplayFormatForLetterExport(Date input) {
		 if (input!= null){
			 Format formatter;
			 formatter = new SimpleDateFormat("d/M/yyyy");
			 return formatter.format(input);
		 }else{
			 return "";
		 }
   }
	*/
	public static string getChineseFormatDate(DateTime? date){
		
		if(date == null){
			return "";
        }
        return date.Value.Year + "年" + date.Value.Month + "月" + date.Value.Day + "日";
        /*
        Calendar calendar = Calendar.getInstance();
		calendar.setTime(date);
		int year = calendar.get(java.util.Calendar.YEAR);
		int month = calendar.get(java.util.Calendar.MONTH)+1;
		int dates = calendar.get(java.util.Calendar.DATE);
		return  year + "年" + month+"月" +dates+"日";*/
	}
	public static string getEnglishFormatDate(DateTime? date){
		if(date == null){
			return "";
		}
        return date.Value.Day + " " + getFullMonth(date.Value.Month) + " " + date.Value.Year;
        /*
        Calendar calendar = Calendar.getInstance();
		calendar.setTime(date);
		int year = calendar.get(java.util.Calendar.YEAR);
		int month = calendar.get(java.util.Calendar.MONTH)+1;
		int dates = calendar.get(java.util.Calendar.DATE);
		return  dates +" "+getFullMonth(month) + " "+ year ;
		*/
	}
	/*
	public static string getEnglishFormatDateWithDayOfWeek(Date date){
		 if (date!= null){
			 Format formatter;
			 formatter = new SimpleDateFormat(FULL_ENGLISH_DATE_WITH_DAY_OF_WEEK, Locale.ENGLISH);
			 return formatter.format(date);
		 }else{
			 return "";
		 }
		
	}
	public static string getEnglishFormatDateWithoutDay(Date date){
		 if (date!= null){
			 Format formatter;
			 formatter = new SimpleDateFormat(FULL_ENGLISH_DATE_WITHOUT_DAY, Locale.ENGLISH);
			 return formatter.format(date);
		 }else{
			 return "";
		 }
		
	}
	
	
	public static string diffInMins(Date start, DateTime end){
		
		return ""+((end.getTime() - start.getTime())/MINUTE);
	}
	
	public static long diffInDays(Date start, DateTime end){
		
		return ((end.getTime() - start.getTime())/DAY);
	}

	public static DateTime addTime(Date date, string time) {
		long timeLong = date.getTime();
		
		if (time!=null &&!time.equals("")){
			DateFormat dateformat = new SimpleDateFormat(DATETIME_DISPLAY_FORMAT, Locale.ENGLISH);
			 DateTime result = date;
			try{ 
				if (time.length()==7)
					time = "0"+time;
				time = "01/01/1970 "+time;
				long addLong = dateformat.parse(time).getTime()+8*60*60*1000;
				result = new Date(timeLong + addLong);
			 }catch(Exception e){
				 return result;
			 }
			 return result;
		 }else{
			 return null;
		 }
	}

	public static DateTime addDuration(Date date, Object object) {
		long dateLong = date.getTime();
		try{ 
			dateLong = (Long.parseLong(object.toString())*MINUTE) + dateLong;

		 }catch(Exception e){
			 return date;
		 }
		return new Date(dateLong);
	}
	
	public static DateTime addYear(Date date, int numberofYear){
		
		if (numberofYear == 0){
			return date;
		}
		
		if(date == null){
			date = new Date();
			
		}
		Calendar cal = Calendar.getInstance();
		cal.setTime(date);
		cal.add(Calendar.DATE, -1);
		cal.add(Calendar.YEAR, numberofYear);
		return cal.getTime();
	}
	public static DateTime addYearForProfessional(Date date, int numberofYear){
		
		if (numberofYear == 0){
			return date;
		}
		
		if(date == null){
			date = new Date();
			
		}
		Calendar cal = Calendar.getInstance();
		cal.setTime(date);
//		cal.add(Calendar.DATE, -1);
		cal.add(Calendar.YEAR, numberofYear);
		return cal.getTime();
	}
    */
	public static string getFullMonth(int month){
		switch (month) {
            case 1:  return "January";
            case 2:  return "February";
            case 3:  return "March";
            case 4:  return "April";
            case 5:  return "May"; 
            case 6:  return "June";
            case 7:  return "July";
            case 8:  return "August"; 
            case 9:  return "September";
            case 10: return "October"; 
            case 11: return "November";
            case 12: return "December";
            default: return "Invalid month.";
        }

	}
	
	public static int compareDate(DateTime? d1, DateTime d2) {
        if (d1 == null) return 0;
		short vl = 1;
        DateTime t1 = new DateTime(d1.Value.Year, d1.Value.Month, d1.Value.Day);
        DateTime t2 = new DateTime(d2.Year, d2.Month, d2.Day);

        long tickdiff = t1.Ticks - t2.Ticks;
        if (tickdiff > 0) return 2;
        else return 0;
        /*GregorianCalendar gc = new GregorianCalendar();
		gc.setTime(d1);
		int year = gc.get(GregorianCalendar.YEAR);
		int month = gc.get(GregorianCalendar.MONTH);
		int day = gc.get(GregorianCalendar.DAY_OF_MONTH);
		gc.setTime(d2);
		int tempYear = gc.get(GregorianCalendar.YEAR);
		int tempMonth = gc.get(GregorianCalendar.MONTH);
		int tempDay = gc.get(GregorianCalendar.DAY_OF_MONTH);
		if (year != tempYear) {
			if (year > tempYear)
				vl = 2;
			else
				vl = 0;
		} else {
			if (month != tempMonth) {
				if (month > tempMonth)
					vl = 2;
				else
					vl = 0;
			} else {
				if (day != tempDay) {
					if (day > tempDay)
						vl = 2;
					else
						vl = 0;
				}
			}
		}
		return vl;
        */
	}
	
	
}
