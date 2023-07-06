using berkebutunleme.Models;
using berkebutunleme.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace berkebutunleme.Controllers
{
    public class ServisController : ApiController
    {
        Database1Entities5 db = new Database1Entities5();
        SonucModel sonuc = new SonucModel();


        #region Kişiler

        [HttpGet]
        [Route("api/kisilerliste")]
        public List<KisilerModel> KisilerListe()
        {
            List<KisilerModel> liste = db.Kisiler.Select(x => new
                KisilerModel()
            {
                kisiId = x.kisiId,
                kisiNo = x.kisiNo,
                kisiAd = x.kisiAd,
                kisiMail = x.kisiMail,
                kisiYas = x.kisiYas

            }).ToList();

            return liste;
        }

        [HttpGet]
        [Route("api/kisilerbyid/{Kisiid}")]

        public KisilerModel KisilerById(string kisiId)
        {
            KisilerModel kayit = db.Kisiler.Where(s => s.kisiId ==
              kisiId).Select(x => new KisilerModel()
              {

                  kisiId = x.kisiId,
                  kisiNo = x.kisiNo,
                  kisiAd = x.kisiAd,
                  kisiMail = x.kisiMail,
                  kisiYas = x.kisiYas

              }).SingleOrDefault();
            return kayit;
        }

        [HttpPost]
        [Route("api/kisilerekle")]
        public SonucModel KisilerEkle(KisilerModel model)
        {
            if (db.Kisiler.Count(c => c.kisiNo == model.kisiNo) > 0)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Girilen Kişi Numarasi Kayitlidir!";
                return sonuc;
            }
            Kisiler yeni = new Kisiler();
            yeni.kisiId = Guid.NewGuid().ToString();
            yeni.kisiNo = model.kisiNo;
            yeni.kisiAd = model.kisiAd;
            yeni.kisiMail = model.kisiMail;
            yeni.kisiYas = model.kisiYas;

            db.Kisiler.Add(yeni);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Kişi Eklendi";


            return sonuc;
        }

        [HttpPut]
        [Route("api/kisilerduzenle")]

        public SonucModel KisilerDuzenle(KisilerModel model)
        {
            Kisiler kayit = db.Kisiler.Where(s => s.kisiId == model.kisiId).FirstOrDefault();
            if (kayit == null)
            {

                sonuc.islem = false;
                sonuc.mesaj = "Kayit Bulunamadi!";
                return sonuc;

            }

            kayit.kisiNo = model.kisiNo;
            kayit.kisiAd = model.kisiAd;
            kayit.kisiMail = model.kisiMail;
            kayit.kisiYas = model.kisiYas;

            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Kişiler Güncellendi";

            return sonuc;
        }

        [HttpDelete]
        [Route("api/kisilersil/{kisiId}")]

        public SonucModel KisilerSil(string kisiId)
        {
            Kisiler kayit = db.Kisiler.Where(s => s.kisiId == kisiId).SingleOrDefault();

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Kayıt Bulunamadı";
                return sonuc;
            }

            if (db.Kayit.Count(s => s.kayitKisiId == kisiId) > 0)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Kişi Üzerinde Grup Kaydı Olduğu İçin Kişi Silinemez!";
                return sonuc;
            }

            db.Kisiler.Remove(kayit);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Kişi Silindi";

            return sonuc;
        }

        #endregion

        #region Kayit

        [HttpGet]
        [Route("api/kisilermesajliste/{kisiId}")]

        public List<KayitModel> KisilerMesajListe(string kisiId)
        {

            List<KayitModel> liste = db.Kayit.Where(s => s.kayitKisiId == kisiId).Select(x => new
              KayitModel()
            {
                kayitId = x.kayitId,
                kayitMesajId = x.kayitMesajId,
                kayitKisiId = x.kayitKisiId,

            }).ToList();

            foreach (var kayit in liste)
            {
                kayit.kisiBilgi = KisilerById(kayit.kayitKisiId);
                kayit.mesajBilgi = MesajById(kayit.kayitMesajId);
            }

            return liste;
        }


        [HttpGet]
        [Route("api/mesajkisilerliste/{mesajId}")]

        public List<KayitModel> MesajKisilerListe(string mesajId)
        {

            List<KayitModel> liste = db.Kayit.Where(s => s.kayitMesajId == mesajId).Select(x => new
              KayitModel()
            {
                kayitId = x.kayitId,
                kayitMesajId = x.kayitMesajId,
                kayitKisiId = x.kayitKisiId,

            }).ToList();

            foreach (var kayit in liste)
            {
                kayit.kisiBilgi = KisilerById(kayit.kayitKisiId);
                kayit.mesajBilgi = MesajById(kayit.kayitMesajId);
            }

            return liste;
        }


        [HttpPost]
        [Route("api/kayitekle")]

        public SonucModel KayitEkle(KayitModel model)
        {
            if (db.Kayit.Count(s => s.kayitMesajId == model.kayitMesajId && s.kayitKisiId ==
            model.kayitKisiId) > 0)

            {
                sonuc.islem = false;
                sonuc.mesaj = "Bu kişiye aynı mesaj daha önce gönderilmiştir";
                return sonuc;
            }

            Kayit yeni = new Kayit();
            yeni.kayitId = Guid.NewGuid().ToString();
            yeni.kayitKisiId = model.kayitKisiId;
            yeni.kayitMesajId = model.kayitMesajId;

            db.Kayit.Add(yeni);
            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Mesaj Gönderildi";

            return sonuc;
        }


        [HttpDelete]
        [Route("api/kayitsil/{kayitId}")]


        public SonucModel KayitSil(string kayitId)
        {
            Kayit kayit = db.Kayit.Where(s => s.kayitId == kayitId).SingleOrDefault();

            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Kayıt Bulunamadı!";
                return sonuc;
            }

            db.Kayit.Remove(kayit);
            db.SaveChanges();
            sonuc.islem = true;
            sonuc.mesaj = "Mesaj Silindi";
            return sonuc;

        }

        #endregion

        #region Mesaj

        [HttpGet]
        [Route("api/mesajliste")]
        public List<MesajModel> MesajListe()
        {
            List<MesajModel> liste = db.Mesaj.Select(x => new MesajModel()
            {
                mesajId = x.mesajId,
                mesajText = x.mesajText,
                bulkMesaj = x.bulkMesaj,
                grupId = x.grupId,
                kisiAd = x.Kisiler.kisiAd,
                kimdenId = x.kimdenId,
                kimeId = x.kimeId,

            }).ToList();

            return liste;
        }
        [HttpGet]
        [Route("api/mesajlistesoneklenenler/{s}")]
        public List<MesajModel> MesajListeSonEklenenler(int s)
        {
            List<MesajModel> liste = db.Mesaj.OrderByDescending(o => o.mesajId).Take(s).Select(x => new MesajModel()
            {
                mesajId = x.mesajId,
                mesajText = x.mesajText,
                bulkMesaj = x.bulkMesaj,
                grupId = x.grupId,
                kisiAd = x.Kisiler.kisiAd,
                kimdenId = x.kimdenId,
                kimeId = x.kimeId,

            }).ToList();

            return liste;
        }

        [HttpGet]
        [Route("api/mesajbyid/{mesajId}")]
        public MesajModel MesajById(string mesajId)
        {
            MesajModel kayit = db.Mesaj.Where(s => s.mesajId == mesajId).Select(x => new MesajModel()
            {
                mesajId = x.mesajId,
                mesajText = x.mesajText,
                bulkMesaj = x.bulkMesaj,
                grupId = x.grupId,
                kisiAd = x.Kisiler.kisiAd,
                kimdenId = x.kimdenId,
                kimeId = x.kimeId,
            }).SingleOrDefault();
            return kayit;
        }

        [HttpPost]
        [Route("api/mesajekle")]
        public SonucModel MesajEkle(MesajModel model)
        {

            Mesaj yeni = new Mesaj();
            yeni.mesajId = model.mesajId;
            yeni.mesajText = model.mesajText;
            yeni.bulkMesaj = model.bulkMesaj;
            yeni.grupId = model.grupId;
            yeni.kimdenId = model.kimdenId;
            yeni.kimeId = model.kimeId;


            db.Mesaj.Add(yeni);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Mesaj Gönderildi";
            return sonuc;
        }


        [HttpDelete]
        [Route("api/mesajsil/{mesajId}")]
        public SonucModel MesajSil(string mesajId)
        {
            Mesaj kayit = db.Mesaj.Where(s => s.mesajId == mesajId).SingleOrDefault();
            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Kayıt Bulunamadı!";
                return sonuc;
            }


            db.Mesaj.Remove(kayit);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Mesaj Silindi";
            return sonuc;
        }
        #endregion

        #region Grup

        [HttpGet]
        [Route("api/grupliste")]
        public List<GrupModel> GrupListe()
        {
            List<GrupModel> liste = db.Grup.Select(x => new GrupModel()
            {
                grupId = x.grupId,
                grupAd = x.grupAd,

            }).ToList();

            return liste;
        }
        [HttpGet]
        [Route("api/gruplistesoneklenenler/{s}")]
        public List<GrupModel> GrupListeSonEklenenler(int s)
        {
            List<GrupModel> liste = db.Grup.OrderByDescending(o => o.grupId).Take(s).Select(x => new GrupModel()
            {
                grupId = x.grupId,
                grupAd = x.grupAd,

            }).ToList();

            return liste;
        }
        [HttpGet]
        [Route("api/Gruplistebykatid/{katId}")]
        public List<GrupModel> GrupListeByMesajId(int mesajId)
        {
            List<GrupModel> liste = db.Grup.Select(x => new GrupModel()
            {
                grupId = x.grupId,
                grupAd = x.grupAd,
            }).ToList();

            return liste;
        }
        [HttpGet]
        [Route("api/gruplistebykullaniciid/{kullaniciId}")]
        public List<GrupModel> KullaniciListeByKullaniciId(int kullaniciId)
        {
            List<GrupModel> liste = db.Grup.Select(x => new GrupModel()
            {
                grupId = x.grupId,
                grupAd = x.grupAd,

            }).ToList();

            return liste;
        }

        [HttpGet]
        [Route("api/grupbyid/{grupId}")]
        public GrupModel GrupById(string grupId)
        {
            GrupModel kayit = db.Grup.Where(s => s.grupId == grupId).Select(x => new GrupModel()
            {
                grupId = x.grupId,
                grupAd = x.grupAd,
            }).SingleOrDefault();
            return kayit;
        }

        [HttpPost]
        [Route("api/grupekle")]
        public SonucModel GrupEkle(GrupModel model)
        {
            if (db.Grup.Count(s => s.grupAd == model.grupAd) > 0)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Girilen Grup Başlığı Kayıtlıdır!";
                return sonuc;
            }

            Grup yeni = new Grup();
            yeni.grupId = model.grupId;
            yeni.grupAd = model.grupAd;
            db.Grup.Add(yeni);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Grup Eklendi";
            return sonuc;
        }

        [HttpPut]
        [Route("api/grupduzenle")]
        public SonucModel GrupDuzenle(GrupModel model)
        {
            Grup kayit = db.Grup.Where(s => s.grupId == model.grupId).SingleOrDefault();
            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Kayıt Bulunamadı!";
                return sonuc;
            }
            kayit.grupId = model.grupId;
            kayit.grupAd = model.grupAd;
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Grup Düzenlendi";
            return sonuc;

        }

        [HttpDelete]
        [Route("api/grupsil/{grupId}")]
        public SonucModel GrupSil(string grupId)
        {
            Grup kayit = db.Grup.Where(s => s.grupId == grupId).SingleOrDefault();
            if (kayit == null)
            {
                sonuc.islem = false;
                sonuc.mesaj = "Kayıt Bulunamadı!";
                return sonuc;
            }

            db.Grup.Remove(kayit);
            db.SaveChanges();

            sonuc.islem = true;
            sonuc.mesaj = "Grup Silindi";
            return sonuc;
        }

        #endregion

    }
}
