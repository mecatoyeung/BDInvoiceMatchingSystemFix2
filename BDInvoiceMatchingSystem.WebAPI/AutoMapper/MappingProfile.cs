using AutoMapper;
using BDInvoiceMatchingSystem.WebAPI.Enums;
using BDInvoiceMatchingSystem.WebAPI.Forms;
using BDInvoiceMatchingSystem.WebAPI.Models;
using BDInvoiceMatchingSystem.WebAPI.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BDInvoiceMatchingSystem.WebAPI.AutoMapper
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerViewModel>().
                ForMember(dest => dest.ApproximateNames, 
                    opt => opt.MapFrom(src => src.CustomerApproximateMappings.Select(m => m.ApproximateValue)));
            CreateMap<FileSourceCreateForm, FileSource>().
                ForMember(dest => dest.NextCaptureDateTime,
                    opt => opt.MapFrom(src => DateTime.Now.AddSeconds(src.IntervalInSeconds)));
            CreateMap<PriceRebate, PriceRebateListViewModel>();
            CreateMap<PriceRebateItem, PriceRebateItemListViewModel>().
                ForMember(dest => dest.PriceRebateID,
                    opt => opt.MapFrom(src => src.PriceRebate.ID)).
                ForMember(dest => dest.ExcelFilename,
                    opt => opt.MapFrom(src => src.PriceRebate.ExcelFilename)).
                ForMember(dest => dest.UploadedDateTime,
                    opt => opt.MapFrom(src => src.PriceRebate.UploadedDateTime));
            CreateMap<DocumentFromCashew, DocumentFromCashewViewModel>();
            CreateMap<DocumentFromCashewItem, DocumentFromCashewItemListViewModel>().
                ForMember(dest => dest.ID,
                    opt => opt.MapFrom(src => src.ID)).
                ForMember(dest => dest.DocumentFromCashewID,
                    opt => opt.MapFrom(src => src.DocumentFromCashew.ID)).
                ForMember(dest => dest.DocumentFromCashew,
                    opt => opt.MapFrom(src => new DocumentFromCashewViewModel
                    {
                        DocumentClass = src.DocumentFromCashew.DocumentClass,
                        DocumentCreatedFrom = src.DocumentFromCashew.DocumentCreatedFrom,
                        PDFFilename = src.DocumentFromCashew.PDFFilename,
                        CSVFilename = src.DocumentFromCashew.CSVFilename,
                        DocumentNo = src.DocumentFromCashew.DocumentNo,
                        DocumentDate = src.DocumentFromCashew.DocumentDate,
                        DeliveryDate = src.DocumentFromCashew.DeliveryDate,
                        CustomerCode = src.DocumentFromCashew.CustomerCode,
                        CustomerName = src.DocumentFromCashew.CustomerName,
                        CustomerAddress = src.DocumentFromCashew.CustomerAddress,
                        UploadedDateTime = src.DocumentFromCashew.UploadedDateTime
                    })
                );
            CreateMap<DocumentFromCashewItemPagination, DocumentFromCashewItemPaginationViewModel>();
        }
    }
}
