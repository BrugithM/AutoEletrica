using MediatR;
using SgaAutoEletrica.Application.Features.MarcasVeiculo.DTOs;

namespace SgaAutoEletrica.Application.Features.MarcasVeiculo.Queries;

public class ListarMarcasVeiculoQuery : IRequest<List<MarcaVeiculoDTO>> { public bool? Ativo { get; set; } = true; }