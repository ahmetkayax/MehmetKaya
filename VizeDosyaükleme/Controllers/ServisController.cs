using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using VizeDosyaükleme.Models;
using VizeDosyaükleme.ViewModel;

namespace VizeDosyaükleme.Controllers
{
    public class ServisController : ApiController
    {

        public class ServissController : ApiController
        {

            DB05Entities db = new DB05Entities();
            SonucModel sonuc = new SonucModel();

            #region Ders
            [HttpGet]
            [Route("api/derslistele")]

            public List<DersModel> DersListe()
            {
                List<DersModel> liste = db.Ders.Select(x => new DersModel()
                {
                    dersId = x.dersId,
                    dersAdi = x.dersAdi,
                    dersBolumu = x.dersBolumu,
                    dersKodu = x.dersKodu
                }
                ).ToList();
                return liste;
            }

            [HttpGet]
            [Route("api/dersbyid/{dersId}")]
            public DersModel DersById(int dersId)
            {
                DersModel kayit = db.Ders.Where(s => s.dersId == dersId).Select(x => new DersModel()
                {
                    dersId = x.dersId,
                    dersAdi = x.dersAdi,
                    dersBolumu = x.dersBolumu,
                    dersKodu = x.dersKodu
                }).FirstOrDefault();
                return kayit;
            }
            [HttpPost]
            [Route("api/dersekle")]
            public SonucModel DersEkle(DersModel model)
            {
                if (db.Ders.Count(s => s.dersKodu == model.dersKodu) > 0)
                {
                    sonuc.islem = false;
                    sonuc.mesaj = "Girilen Ders Kayıtlıdır.";
                    return sonuc;
                }
                Ders yeni = new Ders();

                yeni.dersKodu = model.dersKodu;
                yeni.dersAdi = model.dersAdi;
                yeni.dersBolumu = model.dersBolumu;
                db.Ders.Add(yeni);
                db.SaveChanges();
                sonuc.islem = true;
                sonuc.mesaj = "Ders Eklendi.";

                return sonuc;

            }
            [HttpPut]
            [Route("api/dersduzenle")]

            public SonucModel DersDuzenle(DersModel model)
            {
                Ders kayit = db.Ders.Where(s => s.dersId == model.dersId).FirstOrDefault();
                if (kayit == null)
                {
                    sonuc.islem = false;
                    sonuc.mesaj = "Kayıt Bulunamadı!";
                    return sonuc;
                }

                kayit.dersKodu = model.dersKodu;
                kayit.dersAdi = model.dersAdi;
                kayit.dersBolumu = model.dersBolumu;
                db.SaveChanges();

                sonuc.islem = true;
                sonuc.mesaj = "Kayıt Düzenlendi.";
                return sonuc;
            }

            [HttpDelete]
            [Route("api/derssil/{dersId}")]

            public SonucModel DersSil(int dersId)
            {
                Ders kayit = db.Ders.Where(s => s.dersId == dersId).FirstOrDefault();
                if (kayit == null)
                {
                    sonuc.islem = false;
                    sonuc.mesaj = "Ders Bulunamadı!";
                    return sonuc;
                }

                if (db.Konu.Count(s => s.dersById == dersId) > 0)
                {
                    sonuc.islem = false;
                    sonuc.mesaj = "Derse Kayıtlı Öğrenci Olduğu İçin Ders Silinemez.";
                }

                db.Ders.Remove(kayit);
                db.SaveChanges();
                sonuc.islem = true;
                sonuc.mesaj = "Ders Silindi.";
                return sonuc;
            }
            #endregion

            #region Dosyalar




            [HttpPost]
            [Route("api/dosyayukle")]
            public IHttpActionResult DosyaYukle()
            {
                try
                {
                    var httpRequest = HttpContext.Current.Request;

                    if (httpRequest.Files.Count > 0)
                    {
                        foreach (string file in httpRequest.Files)
                        {
                            var postedFile = httpRequest.Files[file];
                            var filePath = HttpContext.Current.Server.MapPath("~/Dosyalar" + postedFile.FileName);
                            postedFile.SaveAs(filePath);
                        }
                        return Ok("Dosya yüklendi.");
                    }
                    else
                    {
                        return BadRequest("Yüklenen dosya yok.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Dosya yükleme sırasında hata oluştu: " + ex.Message);
                }
            }

            [HttpGet]
            [Route("api/dosyalistele")]
            public IHttpActionResult DosyaListele()
            {
                try
                {
                    var filePaths = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Dosyalar"));
                    var dosyaListesi = new List<string>();
                    foreach (var filePath in filePaths)
                    {
                        dosyaListesi.Add(Path.GetFileName(filePath));
                    }
                    return Ok(dosyaListesi);
                }
                catch (Exception ex)
                {
                    return BadRequest("Dosya listeleme sırasında hata oluştu: " + ex.Message);
                }
            }


            [HttpPut]
            [Route("api/dosyaguncelle")]
            public IHttpActionResult DosyaGuncelle(string dosyaAdi)
            {
                try
                {
                    var httpRequest = HttpContext.Current.Request;

                    if (httpRequest.Files.Count > 0)
                    {
                        var postedFile = httpRequest.Files[0];
                        var filePath = HttpContext.Current.Server.MapPath("~/Dosyalar/" + dosyaAdi);
                        postedFile.SaveAs(filePath);
                        return Ok("Dosya güncellendi.");
                    }
                    else
                    {
                        return BadRequest("Güncellenecek dosya yok.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Dosya güncelleme sırasında hata oluştu: " + ex.Message);
                }
            }

            [HttpDelete]
            [Route("api/dosyasil")]
            public IHttpActionResult DosyaSil(string dosyaAdi)
            {
                try
                {
                    var filePath = HttpContext.Current.Server.MapPath("~/Dosyalar/" + dosyaAdi);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        return Ok("Dosya silindi.");
                    }
                    else
                    {
                        return BadRequest("Silinecek dosya yok.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Dosya silme sırasında hata oluştu: " + ex.Message);
                }
            }
            [HttpGet]
            [Route("api/dosyaindir")]
            public IHttpActionResult DosyaIndir(string dosyaAdi)
            {
                try
                {
                    var filePath = HttpContext.Current.Server.MapPath("~/Dosyalar/" + dosyaAdi);
                    if (File.Exists(filePath))
                    {
                        var fileBytes = File.ReadAllBytes(filePath);
                        var fileContent = new ByteArrayContent(fileBytes);
                        var fileExtension = Path.GetExtension(filePath);
                        var mimeType = MimeMapping.GetMimeMapping(fileExtension);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                        var response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = fileContent;
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        response.Content.Headers.ContentDisposition.FileName = dosyaAdi;
                        return ResponseMessage(response);
                    }
                    else
                    {
                        return BadRequest("İndirilecek dosya yok.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Dosya indirme sırasında hata oluştu: " + ex.Message);
                }
            }
            #endregion


        
    }

    }
}