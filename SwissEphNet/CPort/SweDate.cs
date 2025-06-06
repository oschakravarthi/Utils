﻿/*
   This is a port of the Swiss Ephemeris Free Edition, Version 2.00.00
   of Astrodienst AG, Switzerland from the original C Code to .Net. For
   copyright see the original copyright notices below and additional
   copyright notes in the file named LICENSE, or - if this file is not
   available - the copyright notes at http://www.astro.ch/swisseph/ and
   following.
   
   For any questions or comments regarding this port, you should
   ONLY contact me and not Astrodienst, as the Astrodienst AG is not involved
   in this port in any way.

   Yanos : ygrenier@ygrenier.com
*/

/*********************************************************
  $Header: /home/dieter/sweph/RCS/swedate.c,v 1.75 2009/04/08 07:17:29 dieter Exp $
  version 15-feb-89 16:30
  
  swe_date_conversion()
  swe_revjul()
  swe_julday()

************************************************************/
/* Copyright (C) 1997 - 2008 Astrodienst AG, Switzerland.  All rights reserved.
  
  License conditions
  ------------------

  This file is part of Swiss Ephemeris.
  
  Swiss Ephemeris is distributed with NO WARRANTY OF ANY KIND.  No author
  or distributor accepts any responsibility for the consequences of using it,
  or for whether it serves any particular purpose or works at all, unless he
  or she says so in writing.  

  Swiss Ephemeris is made available by its authors under a dual licensing
  system. The software developer, who uses any part of Swiss Ephemeris
  in his or her software, must choose between one of the two license models,
  which are
  a) GNU public license version 2 or later
  b) Swiss Ephemeris Professional License
  
  The choice must be made before the software developer distributes software
  containing parts of Swiss Ephemeris to others, and before any public
  service using the developed software is activated.

  If the developer choses the GNU GPL software license, he or she must fulfill
  the conditions of that license, which includes the obligation to place his
  or her whole software project under the GNU GPL or a compatible license.
  See http://www.gnu.org/licenses/old-licenses/gpl-2.0.html

  If the developer choses the Swiss Ephemeris Professional license,
  he must follow the instructions as found in http://www.astro.com/swisseph/ 
  and purchase the Swiss Ephemeris Professional Edition from Astrodienst
  and sign the corresponding license contract.

  The License grants you the right to use, copy, modify and redistribute
  Swiss Ephemeris, but only under certain conditions described in the License.
  Among other things, the License requires that the copyright notices and
  this notice be preserved on all copies.

  Authors of the Swiss Ephemeris: Dieter Koch and Alois Treindl

  The authors of Swiss Ephemeris have no control or influence over any of
  the derived works, i.e. over software or services created by other
  programmers which use Swiss Ephemeris functions.

  The names of the authors or of the copyright holder (Astrodienst) must not
  be used for promoting any software, product or service which uses or contains
  the Swiss Ephemeris. This copyright notice is the ONLY place where the
  names of the authors can legally appear, except in cases where they have
  given special permission in writing.

  The trademarks 'Swiss Ephemeris' and 'Swiss Ephemeris inside' may be used
  for promoting such software, products or services.
*/
namespace SwissEphNet.CPort
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    partial class SweDate : BaseCPort
    {
        public SweDate(SwissEph se)
            : base(se) {
        }

        /*
          swe_date_conversion():
          This function converts some date+time input {y,m,d,uttime}
          into the Julian day number tjd.
          The function checks that the input is a legal combination
          of dates; for illegal dates like 32 January 1993 it returns ERR
          but still converts the date correctly, i.e. like 1 Feb 1993.
          The function is usually used to convert user input of birth data
          into the Julian day number. Illegal dates should be notified to the user.

          Be aware that we always use astronomical year numbering for the years
          before Christ, not the historical year numbering.
          Astronomical years are done with negative numbers, historical
          years with indicators BC or BCE (before common era).
          Year 0 (astronomical)  	= 1 BC historical.
          year -1 (astronomical) 	= 2 BC
          etc.
          Many users of Astro programs do not know about this difference.

          Return: OK or ERR (for illegal date)
        *********************************************************/

        bool init_leapseconds_done = false;


        public int swe_date_conversion(int y,
                     int m,
                     int d,		/* day, month, year */
                     double uttime, 	/* UT in hours (decimal) */
                     char c,		/* calendar g[regorian]|j[ulian] */
                     ref double tjd) {
            int rday = 0, rmon = 0, ryear = 0;
            double rut, jd;
            int gregflag = SwissEph.SE_JUL_CAL;
            if (c == 'g')
                gregflag = SwissEph.SE_GREG_CAL;
            rut = uttime;		/* hours UT */
            jd = swe_julday(y, m, d, rut, gregflag);
            swe_revjul(jd, gregflag, ref ryear, ref rmon, ref rday, ref rut);
            tjd = jd;
            if (rmon == m && rday == d && ryear == y) {
                return SwissEph.OK;
            } else {
                return SwissEph.ERR;
            }
        }

        /*************** swe_julday ********************************************
         * This function returns the absolute Julian day number (JD)
         * for a given calendar date.
         * The arguments are a calendar date: day, month, year as integers,
         * hour as double with decimal fraction.
         * If gregflag = SE_GREG_CAL (1), Gregorian calendar is assumed,
         * if gregflag = SE_JUL_CAL (0),Julian calendar is assumed.
  
         The Julian day number is a system of numbering all days continously
         within the time range of known human history. It should be familiar
         to every astrological or astronomical programmer. The time variable
         in astronomical theories is usually expressed in Julian days or
         Julian centuries (36525 days per century) relative to some start day;
         the start day is called 'the epoch'.
         The Julian day number is a double representing the number of
         days since JD = 0.0 on 1 Jan -4712, 12:00 noon (in the Julian calendar).
 
         Midnight has always a JD with fraction .5, because traditionally
         the astronomical day started at noon. This was practical because
         then there was no change of date during a night at the telescope.
         From this comes also the fact the noon ephemerides were printed
         before midnight ephemerides were introduced early in the 20th century.
 
         NOTE: The Julian day number must not be confused with the Julian 
         calendar system.

         Be aware the we always use astronomical year numbering for the years
         before Christ, not the historical year numbering.
         Astronomical years are done with negative numbers, historical
         years with indicators BC or BCE (before common era).
         Year 0 (astronomical)  	= 1 BC
         year -1 (astronomical) 	= 2 BC
         etc.

         Original author: Marc Pottenger, Los Angeles.
         with bug fix for year < -4711   15-aug-88 by Alois Treindl
 
         References: Oliver Montenbruck, Grundlagen der Ephemeridenrechnung,
                     Verlag Sterne und Weltraum (1987), p.49 ff
 
         related functions: swe_revjul() reverse Julian day number: compute the
                           calendar date from a given JD
                        date_conversion() includes test for legal date values
                    and notifies errors like 32 January.
         ****************************************************************/
        public double swe_julday(int year, int month, int day, double hour, int gregflag) {
            double jd;
            double u, u0, u1, u2;
            u = year;
            if (month < 3) u -= 1;
            u0 = u + 4712.0;
            u1 = month + 1.0;
            if (u1 < 4) u1 += 12.0;
            jd = Math.Floor(u0 * 365.25)
               + Math.Floor(30.6 * u1 + 0.000001)
               + day + hour / 24.0 - 63.5;
            if (gregflag == SwissEph.SE_GREG_CAL) {
                u2 = Math.Floor(Math.Abs(u) / 100) - Math.Floor(Math.Abs(u) / 400);
                if (u < 0.0) u2 = -u2;
                jd = jd - u2 + 2;
                if ((u < 0.0) && (u / 100 == Math.Floor(u / 100)) && (u / 400 != Math.Floor(u / 400)))
                    jd -= 1;
            }
            return jd;
        }

        /*** swe_revjul ******************************************************
          swe_revjul() is the inverse function to swe_julday(), see the description
          there.
          Arguments are julian day number, calendar flag (0=julian, 1=gregorian)
          return values are the calendar day, month, year and the hour of
          the day with decimal fraction (0 .. 23.999999).

          Be aware the we use astronomical year numbering for the years
          before Christ, not the historical year numbering.
          Astronomical years are done with negative numbers, historical
          years with indicators BC or BCE (before common era).
          Year  0 (astronomical)  	= 1 BC historical year
          year -1 (astronomical) 	= 2 BC historical year
          year -234 (astronomical) 	= 235 BC historical year
          etc.

          Original author Mark Pottenger, Los Angeles.
          with bug fix for year < -4711 16-aug-88 Alois Treindl
        *************************************************************************/
        public void swe_revjul(double jd, int gregflag,
                 ref int jyear, ref int jmon, ref int jday, ref double jut) {
            double u0, u1, u2, u3, u4;
            u0 = jd + 32082.5;
            if (gregflag == SwissEph.SE_GREG_CAL) {
                u1 = u0 + Math.Floor(u0 / 36525.0) - Math.Floor(u0 / 146100.0) - 38.0;
                if (jd >= 1830691.5) u1 += 1;
                u0 = u0 + Math.Floor(u1 / 36525.0) - Math.Floor(u1 / 146100.0) - 38.0;
            }
            u2 = Math.Floor(u0 + 123.0);
            u3 = Math.Floor((u2 - 122.2) / 365.25);
            u4 = Math.Floor((u2 - Math.Floor(365.25 * u3)) / 30.6001);
            jmon = (int)(u4 - 1.0);
            if (jmon > 12) jmon -= 12;
            jday = (int)(u2 - Math.Floor(365.25 * u3) - Math.Floor(30.6001 * u4));
            jyear = (int)(u3 + Math.Floor((u4 - 2.0) / 12.0) - 4800);
            jut = (jd - Math.Floor(jd + 0.5) + 0.5) * 24.0;
        }

        /* transform local time to UTC or UTC to local time
         *
         * input 
         *   iyear ... dsec     date and time
         *   d_timezone		timezone offset
         * output
         *   iyear_out ... dsec_out
         * 
         * For time zones east of Greenwich, d_timezone is positive.
         * For time zones west of Greenwich, d_timezone is negative.
         * 
         * For conversion from local time to utc, use +d_timezone.
         * For conversion from utc to local time, use -d_timezone.
         */
        public void swe_utc_time_zone(
                Int32 iyear, Int32 imonth, Int32 iday,
                Int32 ihour, Int32 imin, double dsec,
                double d_timezone,
                ref Int32 iyear_out, ref Int32 imonth_out, ref Int32 iday_out,
                ref Int32 ihour_out, ref Int32 imin_out, ref double dsec_out
                ) {
            double tjd, d = 0;
            bool have_leapsec = false;
            double dhour;
            if (dsec >= 60.0) {
                have_leapsec = true;
                dsec -= 1.0;
            }
            dhour = ihour + imin / 60.0 + dsec / 3600.0;
            tjd = swe_julday(iyear, imonth, iday, 0, SwissEph.SE_GREG_CAL);
            dhour -= d_timezone;
            if (dhour < 0.0) {
                tjd -= 1.0;
                dhour += 24.0;
            }
            if (dhour >= 24.0) {
                tjd += 1.0;
                dhour -= 24.0;
            }
            swe_revjul(tjd + 0.001, SwissEph.SE_GREG_CAL, ref iyear_out, ref imonth_out, ref iday_out, ref d);
            ihour_out = (int)dhour;
            d = (dhour - ihour_out) * 60;
            imin_out = (int)d;
            dsec_out = (d - imin_out) * 60;
            if (have_leapsec)
                dsec_out += 1.0;
        }

        /*
         * functions for the handling of UTC
         */

        /* Leap seconds were inserted at the end of the following days:*/
        const int NLEAP_SECONDS = 27;
        const int NLEAP_SECONDS_SPACE = 100;
        int[] leap_seconds = [
        19720630,
        19721231,
        19731231,
        19741231,
        19751231,
        19761231,
        19771231,
        19781231,
        19791231,
        19810630,
        19820630,
        19830630,
        19850630,
        19871231,
        19891231,
        19901231,
        19920630,
        19930630,
        19940630,
        19951231,
        19970630,
        19981231,
        20051231,
        20081231,
        20120630,
        20150630,
        20161231,
        0  /* keep this 0 as end mark */
        ];
        const double J1972 = 2441317.5;
        const int NLEAP_INIT = 10;

        /* Read additional leap second dates from external file, if given.
         */
        int init_leapsec() {
            CFile fp;
            int ndat, ndat_last;
            int tabsiz = 0;
            int i;
            string s;
            //char *sp;
            if (!init_leapseconds_done) {
                var list = new List<int>(leap_seconds);
                while (list.Count < NLEAP_SECONDS_SPACE) list.Add(0);
                leap_seconds = list.ToArray();
                init_leapseconds_done = true;
                tabsiz = NLEAP_SECONDS;
                ndat_last = leap_seconds[NLEAP_SECONDS - 1];
                /* no error message if file is missing */
                string sdummy = null;
                if ((fp = SE.Sweph.swi_fopen(-1, "seleapsec.txt", SE.Sweph.swed.ephepath, ref sdummy)) == null)
                    return NLEAP_SECONDS;
                while ((s = fp.ReadLine()) != null) {
                    s = s.TrimStart(' ', '\t');
                    if (String.IsNullOrEmpty(s) || s.StartsWith("#")) continue;
                    ndat = int.Parse(s);
                    if (ndat <= ndat_last)
                        continue;
                    /* table space is limited. no error msg, if exceeded */
                    if (tabsiz >= NLEAP_SECONDS_SPACE)
                        return tabsiz;
                    leap_seconds[tabsiz] = ndat;
                    tabsiz++;
                }
                if (tabsiz > NLEAP_SECONDS) leap_seconds[tabsiz] = 0; /* end mark */
                fp.Dispose();
                return tabsiz;
            }
            /* find table size */
            tabsiz = 0;
            for (i = 0; i < NLEAP_SECONDS_SPACE; i++) {
                if (leap_seconds[i] == 0)
                    break;
                else
                    tabsiz++;
            }
            return tabsiz;
        }

        /*
         * Input:  Clock time UTC, year, month, day, hour, minute, second (decimal).
         *         gregflag  Calendar flag
         *         serr      error string
         * Output: An array of doubles:
         *         dret[0] = Julian day number TT (ET)
         *         dret[1] = Julian day number UT1
         *
         * Function returns OK or Error.
         *
         * - Before 1972, swe_utc_to_jd() treats its input time as UT1.
         *   Note: UTC was introduced in 1961. From 1961 - 1971, the length of the
         *   UTC second was regularly changed, so that UTC remained very close to UT1.
         * - From 1972 on, input time is treated as UTC.
         * - If delta_t - nleap - 32.184 > 1, the input time is treated as UT1.
         *   Note: Like this we avoid errors greater than 1 second in case that
         *   the leap seconds table (or the Swiss Ephemeris version) is not updated
         *   for a long time.
        */
        public Int32 swe_utc_to_jd(Int32 iyear, Int32 imonth, Int32 iday, Int32 ihour, Int32 imin, double dsec, Int32 gregflag, double[] dret, ref string serr) {
            double tjd_ut1, tjd_et, tjd_et_1972, dhour, d = 0;
            int iyear2 = 0, imonth2 = 0, iday2 = 0;
            int i, j, ndat, nleap, tabsiz_nleap;
            String sdummy = null;
            /* 
             * error handling: invalid iyear etc. 
             */
            tjd_ut1 = swe_julday(iyear, imonth, iday, 0, gregflag);
            swe_revjul(tjd_ut1, gregflag, ref iyear2, ref imonth2, ref iday2, ref d);
            if (iyear != iyear2 || imonth != imonth2 || iday != iday2) {
                serr = C.sprintf("invalid date: year = %d, month = %d, day = %d", iyear, imonth, iday);
                return SwissEph.ERR;
            }
            if (ihour < 0 || ihour > 23
             || imin < 0 || imin > 59
             || dsec < 0 || dsec >= 61
             || (dsec >= 60 && (imin < 59 || ihour < 23 || tjd_ut1 < J1972))) {
                serr = C.sprintf("invalid time: %d:%d:%.2f", ihour, imin, dsec);
                return SwissEph.ERR;
            }
            dhour = ihour + imin / 60.0 + dsec / 3600.0;
            /* 
             * before 1972, we treat input date as UT1 
             */
            if (tjd_ut1 < J1972) {
                dret[1] = swe_julday(iyear, imonth, iday, dhour, gregflag);
                dret[0] = dret[1] + SE.SwephLib.swe_deltat_ex(dret[1], -1, ref sdummy);
                return SwissEph.OK;
            }
            /* 
             * if gregflag = Julian calendar, convert to gregorian calendar 
             */
            if (gregflag == SwissEph.SE_JUL_CAL) {
                gregflag = SwissEph.SE_GREG_CAL;
                swe_revjul(tjd_ut1, gregflag, ref iyear, ref imonth, ref iday, ref d);
            }
            /* 
             * number of leap seconds since 1972: 
             */
            tabsiz_nleap = init_leapsec();
            nleap = NLEAP_INIT; /* initial difference between UTC and TAI in 1972 */
            ndat = iyear * 10000 + imonth * 100 + iday;
            for (i = 0; i < tabsiz_nleap; i++) {
                if (ndat <= leap_seconds[i])
                    break;
                nleap++;
            }
            /*
             * For input dates > today:
             * If leap seconds table is not up to date, we'd better interpret the
             * input time as UT1, not as UTC. How do we find out? 
             * Check, if delta_t - nleap - 32.184 > 0.9
             */
            d = SE.SwephLib.swe_deltat_ex(tjd_ut1, -1, ref sdummy) * 86400.0;
            if (d - nleap - 32.184 >= 1.0) {
                dret[1] = tjd_ut1 + dhour / 24.0;
                dret[0] = dret[1] + SE.SwephLib.swe_deltat_ex(dret[1], -1, ref sdummy);
                return SwissEph.OK;
            }
            /* 
             * if input second is 60: is it a valid leap second ? 
             */
            if (dsec >= 60) {
                j = 0;
                for (i = 0; i < tabsiz_nleap; i++) {
                    if (ndat == leap_seconds[i]) {
                        j = 1;
                        break;
                    }
                }
                if (j != 1) {
                    serr = C.sprintf("invalid time (no leap second!): %d:%d:%.2f", ihour, imin, dsec);
                    return SwissEph.ERR;
                }
            }
            /* 
             * convert UTC to ET and UT1 
             */
            /* the number of days between input date and 1 jan 1972: */
            d = tjd_ut1 - J1972;
            /* SI time since 1972, ignoring leap seconds: */
            d += ihour / 24.0 + imin / 1440.0 + dsec / 86400.0;
            /* ET (TT) */
            tjd_et_1972 = J1972 + (32.184 + NLEAP_INIT) / 86400.0;
            tjd_et = tjd_et_1972 + d + (nleap - NLEAP_INIT) / 86400.0;
            d = SE.SwephLib.swe_deltat_ex(tjd_et, -1, ref sdummy);
            tjd_ut1 = tjd_et - SE.SwephLib.swe_deltat_ex(tjd_et - d, -1, ref sdummy);
            tjd_ut1 = tjd_et - SE.SwephLib.swe_deltat_ex(tjd_ut1, -1, ref sdummy);
            dret[0] = tjd_et;
            dret[1] = tjd_ut1;
            return SwissEph.OK;
        }

        /*
         * Input:  tjd_et   Julian day number, terrestrial time (ephemeris time).
         *         gregfalg Calendar flag
         * Output: UTC year, month, day, hour, minute, second (decimal).
         *
         * - Before 1 jan 1972 UTC, output UT1.
         *   Note: UTC was introduced in 1961. From 1961 - 1971, the length of the
         *   UTC second was regularly changed, so that UTC remained very close to UT1.
         * - From 1972 on, output is UTC.
         * - If delta_t - nleap - 32.184 > 1, the output is UT1.
         *   Note: Like this we avoid errors greater than 1 second in case that
         *   the leap seconds table (or the Swiss Ephemeris version) has not been
         *   updated for a long time.
         */
        public void swe_jdet_to_utc(double tjd_et, Int32 gregflag, ref Int32 iyear, ref Int32 imonth, ref Int32 iday, ref Int32 ihour, ref Int32 imin, ref double dsec) {
            int i;
            int second_60 = 0;
            int iyear2 = 0, imonth2 = 0, iday2 = 0, nleap, ndat, tabsiz_nleap;
            double d, tjd, tjd_et_1972, tjd_ut; double[] dret = new double[10];
            String sdummy = null;
            /* 
             * if tjd_et is before 1 jan 1972 UTC, return UT1
             */
            tjd_et_1972 = J1972 + (32.184 + NLEAP_INIT) / 86400.0;
            d = SE.SwephLib.swe_deltat_ex(tjd_et, -1, ref sdummy);
            tjd_ut = tjd_et - SE.SwephLib.swe_deltat_ex(tjd_et - d, -1, ref sdummy);
            tjd_ut = tjd_et - SE.SwephLib.swe_deltat_ex(tjd_ut, -1, ref sdummy);
            if (tjd_et < tjd_et_1972)
            {
                swe_revjul(tjd_ut, gregflag, ref iyear, ref imonth, ref iday, ref d);
                ihour = (Int32)d;
                d -= ihour;
                d *= 60;
                imin = (Int32)d;
                dsec = (d - imin) * 60.0;
                return;
            }
            /* 
             * minimum number of leap seconds since 1972; we may be missing one leap
             * second
             */
            tabsiz_nleap = init_leapsec();
            swe_revjul(tjd_ut - 1, SwissEph.SE_GREG_CAL, ref iyear2, ref imonth2, ref iday2, ref d);
            ndat = iyear2 * 10000 + imonth2 * 100 + iday2;
            nleap = 0;
            for (i = 0; i < tabsiz_nleap; i++) {
                if (ndat <= leap_seconds[i])
                    break;
                nleap++;
            }
            /* date of potentially missing leapsecond */
            if (nleap < tabsiz_nleap) {
                i = leap_seconds[nleap];
                iyear2 = i / 10000;
                imonth2 = (i % 10000) / 100; ;
                iday2 = i % 100;
                tjd = swe_julday(iyear2, imonth2, iday2, 0, SwissEph.SE_GREG_CAL);
                swe_revjul(tjd + 1, SwissEph.SE_GREG_CAL, ref iyear2, ref imonth2, ref iday2, ref d);
                swe_utc_to_jd(iyear2, imonth2, iday2, 0, 0, 0, SwissEph.SE_GREG_CAL, dret, ref sdummy);
                d = tjd_et - dret[0];
                if (d >= 0) {
                    nleap++;
                } else if (d is < 0 and > -1.0 / 86400.0) {
                    second_60 = 1;
                }
            }
            /*
             * UTC, still unsure about one leap second
             */
            tjd = J1972 + (tjd_et - tjd_et_1972) - ((double)nleap + second_60) / 86400.0;
            swe_revjul(tjd, SwissEph.SE_GREG_CAL, ref iyear, ref imonth, ref iday, ref d);
            ihour = (Int32)d;
            d -= ihour;
            d *= 60;
            imin = (Int32)d;
            dsec = (d - imin) * 60.0 + second_60;
            /*
             * For input dates > today:
             * If leap seconds table is not up to date, we'd better interpret the
             * input time as UT1, not as UTC. How do we find out? 
             * Check, if delta_t - nleap - 32.184 > 0.9
             */
            d = SE.SwephLib.swe_deltat_ex(tjd_et, -1, ref sdummy);
            d = SE.SwephLib.swe_deltat_ex(tjd_et - d, -1, ref sdummy);
            if (d * 86400.0 - (nleap + NLEAP_INIT) - 32.184 >= 1.0) {
                swe_revjul(tjd_et - d, SwissEph.SE_GREG_CAL, ref iyear, ref imonth, ref iday, ref d);
                ihour = (Int32)d;
                d -= ihour;
                d *= 60;
                imin = (Int32)d;
                dsec = (d - imin) * 60.0;
            }
            if (gregflag == SwissEph.SE_JUL_CAL) {
                tjd = swe_julday(iyear, imonth, iday, 0, SwissEph.SE_GREG_CAL);
                swe_revjul(tjd, gregflag, ref iyear, ref imonth, ref iday, ref d);
            }
        }

        /*
         * Input:  tjd_ut   Julian day number, universal time (UT1).
         *         gregfalg Calendar flag
         * Output: UTC year, month, day, hour, minute, second (decimal).
         *
         * - Before 1 jan 1972 UTC, output UT1.
         *   Note: UTC was introduced in 1961. From 1961 - 1971, the length of the
         *   UTC second was regularly changed, so that UTC remained very close to UT1.
         * - From 1972 on, output is UTC.
         * - If delta_t - nleap - 32.184 > 1, the output is UT1.
         *   Note: Like this we avoid errors greater than 1 second in case that
         *   the leap seconds table (or the Swiss Ephemeris version) has not been
         *   updated for a long time.
         */
        public void swe_jdut1_to_utc(double tjd_ut, Int32 gregflag, ref Int32 iyear, ref Int32 imonth, ref Int32 iday, ref Int32 ihour, ref Int32 imin, ref double dsec) {
            String sdummy = null;
            double tjd_et = tjd_ut + SE.SwephLib.swe_deltat_ex(tjd_ut, -1, ref sdummy);
            swe_jdet_to_utc(tjd_et, gregflag, ref iyear, ref imonth, ref iday, ref ihour, ref imin, ref dsec);
        }

    }
}
