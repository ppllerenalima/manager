namespace Manager.Infrastructure.ExternalServices.ClienteSol
{
    public  class ClienteSolService : IClienteSolService
    {
        private readonly ClientesSolClient _clienteSolClient;

        public ClienteSolService(ClientesSolClient clientesSolClient)
        {
            clientesSolClient = clientesSolClient;
        }

        public async Task<BaseResponseGeneric<SunatAuthResponse>> AccessTokenAsync(SunatAuthRequest request, CancellationToken cancellationToken)
        {
            return await _clienteSolClient.AccessTokenAsync(request, cancellationToken);
        }
    }
}
