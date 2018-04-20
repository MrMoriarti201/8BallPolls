<?php
$link = mysqli_connect("$databaseHostname", "$databaseUsername", "$databasePassword", "$databaseDbName", "$databasePort");

if (!$link) {
    die('Connect Error (' . mysqli_connect_errno() . ') '
            . mysqli_connect_error());
}

?>