/opt/mssql/bin/sqlservr &

echo "Waiting for SQL Server..."
until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "ASDkjkfsjd@.DKfj23dk" -Q "SELECT 1" -No &> /dev/null
do
  echo "Not ready yet, retrying..."
  sleep 2
done

echo "SQL Server ready, running init script..."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "ASDkjkfsjd@.DKfj23dk" -d master -i /db-init/init.sql -No

wait